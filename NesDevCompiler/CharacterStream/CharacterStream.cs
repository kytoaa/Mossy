namespace NesDevCompiler.CharacterStream;

public class CharacterStream : ICharacterStream
{
	private string _data;

	public CharacterStream(string data)
	{
		_data = ProcessData(data);
	}

	public string ProcessData(string data)
	{
		string cleanedData = "";
		foreach (string line in data.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
		{
			string final = line;
			if (!final.Contains("//"))
			{
				cleanedData += final;
				continue;
			}

			cleanedData += final.Remove(final.IndexOf("//"));
		}

		cleanedData = cleanedData.Replace("\n", "").Replace("\r", "");

		cleanedData = cleanedData.Replace("	", "");

		return cleanedData;
	}

	public char Peek()
	{
		return _data[0];
	}
	public char Peek(int c)
	{
		return _data[c];
	}
	public char Read()
	{
		char c = _data[0];
		_data = _data.Remove(0, 1);
		return c;
	}
	public char Read(int c)
	{
		char i = _data[c];
		_data.Remove(0, 1);
		return i;
	}
	public bool End()
	{
		return _data.Count() <= 0;
	}

	public object Clone()
	{
		return new CharacterStream(_data);
	}
}