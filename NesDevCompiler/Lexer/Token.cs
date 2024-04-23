namespace NesDevCompiler.Lexer;

public struct Token
{
	public TokenType Type;
	public string Value;

	public Token(TokenType type, string value)
	{
		Type = type;
		Value = value;
	}

	public static explicit operator string(Token token)
	{
		return token.Value;
	}
}