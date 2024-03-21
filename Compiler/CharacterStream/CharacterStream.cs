namespace NesDevCompiler.CharacterStream;

public class CharacterStream : ICharacterStream
{
	private string _data;

	public CharacterStream(string data)
	{
		_data = data;
	}

	public char Peek()
	{
		return _data[0];
	}
	public char Next()
	{
		char c = _data[0];
		_data.Remove(0, 1);
		return c;
	}
	public bool End()
	{
		return _data.Count() <= 0;
	}
}