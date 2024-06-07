using Mossy.CharacterStream;
using System.Diagnostics;
using Mossy.Processors;

namespace Mossy.Lexer;

public class Lexer : ILexer
{
	private ICharacterStream _stream;
	private IEnumerable<ILexerProcessor> _processors;

	public Lexer(ICharacterStream stream, IEnumerable<ILexerProcessor> processors)
	{
		_stream = stream;
		_processors = processors;
	}

	public Token Peek(bool ignoreWhitespace = true, int t = 0)
	{
		ICharacterStream stream = (ICharacterStream)_stream.Clone();
		Token token = GetToken(stream);

		if (ignoreWhitespace)
		{
			while (token.Value == " ")
			{
				token = GetToken(stream);
			}
		}
		for (int i = 0; i < t; i++)
		{
			if (ignoreWhitespace)
			{
				if (token.Value != " ")
				{
					token = GetToken(stream);
				}
				while (token.Value == " ")
				{
					token = GetToken(stream);
				}
			}
			else
			{
				token = GetToken(stream);
			}
		}

		token = ProcessToken(token);

		return token;
	}

	public Token Next(bool ignoreWhitespace = true)
	{
		Token token = GetToken(_stream);

		while (token.Value == " ")
		{
			token = GetToken(_stream);
		}

		token = ProcessToken(token);

		return token;
	}

	public bool End()
	{
		return _stream.End();
	}

	public Token GetToken(ICharacterStream stream)
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

			if (type == TokenType.Punctuation)
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
	}

	public Token ProcessToken(Token token)
	{
		foreach (ILexerProcessor processor in _processors)
		{
			// i feel like theres a more elegant solution than passing in the lexer
			token = processor.Process(token).Invoke(this);
		}
		return token;
	}
}