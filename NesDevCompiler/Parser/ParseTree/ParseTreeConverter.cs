using NesDevCompiler.Lexer;

namespace NesDevCompiler.Parser.ParseTree;

public class ParseTreeConverter
{
	public ParseNode Parse(ILexer lexer)
	{
		Tree tree = new Tree();

		while (!lexer.End())
		{
			Token token = lexer.Next();

			if (token.Value == "(" || token.Value == "{")
			{
				tree.AddChild().SetValue(token.Value);
			}
			if (token.Value == ")" || token.Value == "}")
			{
				tree.SetCurrentToParent().SetValue(token.Value);
			}
			if (token.Type == TokenType.Operator)
			{
				tree.SetValue(token.Value).AddChild();
			}
			if (token.Type == TokenType.Value)
			{
				tree.SetValue(token.Value).SetCurrentToParent();
			}
			if (token.Type == TokenType.Identifier)
			{
				tree.SetValue(token.Value).SetCurrentToParent();
			}
		}

		return tree.Current;
	}
}