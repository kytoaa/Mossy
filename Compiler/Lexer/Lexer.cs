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
		while (true)
		{
			if (stream.End())
				return new Token(TokenType.End, "");
			char c = stream.Peek();
			prev = token;
			token += c;

			TokenType type = TokenIdentifier.GetTokenType(token);

			if (type != TokenType.Identifier)
			{
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
			stream.Read();
		}

		throw new NotImplementedException();
/*		if (stream.End())
			return new Token(TokenType.End, "");
		string token = "";
		int i = 0;

		if (TokenIdentifier.IsPunctuation(stream.Peek().ToString()))
		{
			char punc = consume ? stream.Read() : stream.Peek();
			Token final = new Token(TokenType.Punctuation, punc.ToString());
			return final;
		}
		if (TokenIdentifier.IsOperator(stream.Peek().ToString() + stream.Peek(1)))
		{
			string op = consume ? (stream.Read().ToString() + stream.Read())
								: (stream.Peek().ToString() + stream.Peek(1));

			Token final = new Token(TokenType.Keyword, op);
			return final;
		}
		else if (TokenIdentifier.IsOperator(stream.Peek().ToString()))
		{
			string op = consume ? (stream.Read().ToString())
								: (stream.Peek().ToString());

			Token final = new Token(TokenType.Keyword, op);
			return final;
		}

		while (!stream.End())
		{
			char c = consume ? stream.Read() : stream.Peek(i);
			token += c;

			char next;

			try
			{
				next = stream.Peek(i + 1);
			}
			catch (Exception e)
			{
				return new Token(TokenType.End, "");
			}

			// Token ended: check if its an identifier or keyword
			if (TokenIdentifier.IsPunctuation(next.ToString()) || TokenIdentifier.IsOperator(next.ToString()))
			{
				Debug.WriteLine(token);
				if (TokenIdentifier.IsKeyword(token))
				{
					Token final = new Token(TokenType.Keyword, token);
					return final;
				}
				if (TokenIdentifier.IsValue(token))
				{
					Token final = new Token(TokenType.Value, token);
					return final;
				}
				return new Token(TokenType.Identifier, token);
			}
			i++;
			if (i > 1000)
			{
				throw new Exception("loop looped too much");
			}
		}
		if (TokenIdentifier.IsKeyword(token))
		{
			Token final = new Token(TokenType.Keyword, token);
			return final;
		}
		if (TokenIdentifier.IsValue(token))
		{
			Token final = new Token(TokenType.Value, token);
			return final;
		}
		return new Token(TokenType.Identifier, token);*/
	}
}