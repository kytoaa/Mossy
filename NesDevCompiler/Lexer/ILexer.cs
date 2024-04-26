namespace NesDevCompiler.Lexer;

public interface ILexer
{
	Token Peek(bool ignoreWhitespace = true, int t = 0);
	Token Next(bool ignoreWhitespace = true);
	bool End();
}