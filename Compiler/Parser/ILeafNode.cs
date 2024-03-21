namespace NesDevCompiler.Parser;

public interface ILeafNode : INode
{
	string Instruction { get; }
}