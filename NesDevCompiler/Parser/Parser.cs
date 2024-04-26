using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public class Parser : IParser
{
	public Node Parse(ILexer lexer)
	{
		return ParseAST(lexer);
	}

	private Node ParseAST(ILexer lexer)
	{
		Node root = new Context();
		Tree tree = new Tree(root);

		StateMachine stateMachine = new StateMachine();
		return stateMachine.Parse(lexer, tree);

/*		while (!lexer.End())
		{
			Token token = lexer.Next();

			stateMachine.Parse(token, tree);

			if (tree.current is CompileError)
			{
				return tree.current;
			}
		}*/

		throw new NotImplementedException();
		return root;
	}
}