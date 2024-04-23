using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public class Parser : IParser
{
	public Node Parse(ILexer lexer)
	{
		throw new NotImplementedException();
	}

	private Node ParseAST(ILexer lexer)
	{
		Node root = new GlobalContext();

		while (!lexer.End())
		{
			Token token = lexer.Next();

			if (token.Type == TokenType.Keyword)
			{
				//if (token.Value == "")
			}
		}

		throw new NotImplementedException();
	}
}