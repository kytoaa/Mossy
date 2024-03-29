namespace NesDevCompiler.Parser.ParseTree;

public class BlockOpen : ILeaf
{
	public string Value { get; } = "{";
}