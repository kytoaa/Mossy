using NesDevCompiler.CharacterStream;

namespace NesDevCompiler.Lexer;

public static class TokenIdentifier
{
	public static readonly string[] Punctuation = { "{", "}", "(", ")", "[", "]", ";", ",", " "};
	public static readonly string[] Operators = { "+", "-", "&", "|", "^", "<<", ">>", "==", "!=", "<", ">", "!", "&&", "||", "=" };
	public static readonly string[] Keywords = { "func", "var", "const", "return", "if", "else", "int", "bool", "while" };

	public static bool IsPunctuation(string c) => Punctuation.Contains(c);
	public static bool IsOperator(string c) => Operators.Contains(c);
	public static bool IsKeyword(string c) => Keywords.Contains(c);
	public static bool IsValue(string c)
	{
		if (c == "true" || c == "false")
		{
			return true;
		}
		return int.TryParse(c, out _);
	}

	public static TokenType GetTokenType(string c)
	{
		if (IsPunctuation(c))
			return TokenType.Punctuation;
		if (IsOperator(c))
			return TokenType.Operator;
		if (IsKeyword(c))
			return TokenType.Keyword;
		if (IsValue(c))
			return TokenType.Value;
		return TokenType.Identifier;
	}

	public static bool IsToken(string c)
	{
		if (Punctuation.Contains(c))
			return true;
		if (Operators.Contains(c))
			return true;
		return false;
	}

	public static bool IsToken(ICharacterStream stream)
	{


		throw new NotImplementedException();
	}
}