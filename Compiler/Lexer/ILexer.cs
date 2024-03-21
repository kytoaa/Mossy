namespace NesDevCompiler.Lexer;

public interface ICharacterStream
{
	Token Peek();
	Token Next();
	Token End();
}