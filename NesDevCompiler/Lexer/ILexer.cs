namespace NesDevCompiler.Lexer;

public interface ILexer
{
	Token Peek(bool ignoreWhitespace = true);
	Token Next(bool ignoreWhitespace = true);
	bool End();
}