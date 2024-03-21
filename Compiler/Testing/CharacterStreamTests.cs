using NesDevCompiler.CharacterStream;
using Xunit;

namespace NesDevCompiler.Testing;

public class CharacterStreamTests
{
	[Fact]
	public void TestPeek()
	{
		string value = "foo 1 2 3 bar";
		ICharacterStream stream = new CharacterStream.CharacterStream(value);

		string data = "";

		while (!stream.End())
		{
			data.Append(stream.Next());
		}

		Assert.Equivalent(value, data);
	}
}