namespace NesDevCompiler.Lexer;

public struct Token
{
	public TokenType Type;
	public string Value;

	public Token(TokenType type, string value)
	{
		this.Type = type;
		this.Value = value.Replace("\n", "").Replace("\r", "");
	}
}