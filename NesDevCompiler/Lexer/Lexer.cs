using NesDevCompiler.CharacterStream;
using System.Diagnostics;

namespace NesDevCompiler.Lexer;

public class Lexer : ILexer
{
	private ICharacterStream _stream;

	public Lexer(ICharacterStream stream)
	{
		_stream = stream;
	}

	public Token Peek()
	{
		Token token = GetToken(_stream, false);

		return token;
	}

	public Token Next()
	{
		Token token = GetToken(_stream, true);

		return token;
	}

	public bool End()
	{
		return _stream.End();
	}

	public Token GetToken(ICharacterStream stream, bool consume)
	{
		string token = "";
		string prev = "";
		int i = 0;
		while (true)
		{
			if (stream.End())
				return new Token(TokenType.End, "");
			char c = consume ? stream.Peek() : stream.Peek(i);
			prev = token;
			token += c;

			TokenType type = TokenIdentifier.GetTokenType(token);

			if (type != TokenType.Identifier)
			{
				if (consume)
					stream.Read();
				return new Token(type, token);
			}
			if (!string.IsNullOrEmpty(prev))
			{
				if (TokenIdentifier.GetTokenType(c.ToString()) == TokenType.Punctuation)
				{
					TokenType idenType = TokenIdentifier.GetTokenType(prev);
					return new Token(idenType, prev);
				}
			}
			if (consume)
				stream.Read();
			i++;
		}
	}
}