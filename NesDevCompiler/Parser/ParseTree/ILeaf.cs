namespace NesDevCompiler.Parser.ParseTree;

public interface ILeaf : INode
{
	public string Value { get; }
}