namespace NesDevCompiler.Parser.ParseTree;

public class BlockClose : ILeaf
{
	public string Value { get; } = "}";
}