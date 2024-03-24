namespace NesDevCompiler.Lexer;

public interface ILexer
{
	Token Peek();
	Token Next();
	bool End();
}