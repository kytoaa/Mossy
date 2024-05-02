using NesDevCompiler.CodeConversion;
using Xunit;

namespace NesDevCompiler.Testing;

public class CodeConverterTests
{
	[Fact]
	public void TestHexConverter()
	{
		Assembly6502CodeConverter converter = new Assembly6502CodeConverter();
		Assert.Equal("19", converter.ConvertToHex(25));
	}
}