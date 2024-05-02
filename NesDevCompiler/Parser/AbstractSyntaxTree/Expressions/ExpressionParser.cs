using NesDevCompiler.Lexer;
using System.Security.Cryptography.X509Certificates;

namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class ExpressionParser
{
	public Expression Parse(ILexer lexer)
	{
		ExpressionTree tree = GetExpressionTree(lexer);
		Expression expression = ConvertToAST(null, tree.root);

		return expression;
	}

	private ExpressionTree GetExpressionTree(ILexer lexer)
	{
		ExpressionTree tree = new ExpressionTree();

		int bracketlayer = 0;

		while ((lexer.Peek().Value != "," && lexer.Peek().Value != ";") && !(lexer.Peek().Value == ")" && bracketlayer == 0))
		{
			Token token = lexer.Next();
			if (token.Value == "(")
			{
				tree.AddChild();
				bracketlayer++;
			}
			if (token.Value == ")")
			{
				tree.MoveUp();
				bracketlayer--;
			}
			if (token.Type == TokenType.Identifier || token.Type == TokenType.Value)
			{
				tree.AddChild().SetValue(token);
				if (lexer.Peek().Value == "[")
				{
					lexer.Next();
					tree.SetOffset(lexer.Next().Value);
					lexer.Next();
				}
				tree.MoveUp();
			}
			if (token.Type == TokenType.Operator)
			{
				tree.SetValue(token);
			}
		}
		if (tree.root.value == default)
		{
			tree.root = tree.root.l;
		}
		return tree;
	}

	private Expression ConvertToAST(Expression? parent, ExpressionNode node)
	{
		Expression expression = null;

		if (node.value.Type == TokenType.Identifier)
		{
			expression = new DeclaredVariable(parent, "default", node.value.Value);
			((DeclaredVariable)expression).Offset = int.Parse(node.offset);
			// TODO add offset
		}
		else if (node.value.Type == TokenType.Value)
		{
			expression = new ConstantValue(parent, int.TryParse(node.value.Value, out int _) ? "int" : "bool", node.value.Value);
		}
		else if (node.value.Type == TokenType.Operator)
		{
			int children = 0 + (node.l != null ? 1 : 0) + (node.r != null ? 1 : 0);

			if (children == 2)
			{
				TwoOperandExpression op = new TwoOperandExpression(parent, "default", ConvertToAST(null, node.l), ConvertToAST(null, node.r), node.value.Value);
				expression = op;
			}
			else if (children == 1)
			{
				SingleOperandExpression op = new SingleOperandExpression(parent, "default", ConvertToAST(null, node.l), node.value.Value);
				expression = op;
			}
			else
			{
				throw new Exception("Operator without operands");
			}
		}
		else
		{
			throw new Exception("Mathematical expression parse error");
		}
		return expression;
	}

	private class ExpressionTree
	{
		public ExpressionNode root;
		private ExpressionNode _current;

		public ExpressionTree()
		{
			root = new ExpressionNode(null);
			_current = root;
		}

		public ExpressionTree AddChild()
		{
			ExpressionNode node = new ExpressionNode(_current);
			if (_current.l == null)
			{
				_current.l = node;
			}
			else
			{
				_current.r = node;
			}
			_current = node;
			return this;
		}
		public ExpressionTree SetValue(Token value)
		{
			_current.value = value;
			return this;
		}
		public ExpressionTree SetOffset(string offset)
		{
			_current.offset = offset;
			return this;
		}
		public ExpressionTree MoveUp()
		{
			if (_current != null)
			{
				_current = _current.parent;
			}

			return this;
		}
	}
	private class ExpressionNode
	{
		public ExpressionNode? parent;
		public Token value;
		public ExpressionNode? l;
		public ExpressionNode? r;
		public string offset = "0";

		public ExpressionNode(ExpressionNode parent)
		{
			this.parent = parent;
		}
	}
}