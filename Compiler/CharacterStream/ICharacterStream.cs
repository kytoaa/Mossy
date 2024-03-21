namespace NesDevCompiler.CharacterStream;

public interface ICharacterStream
{
	char Peek();
	char Next();
	bool End();
}