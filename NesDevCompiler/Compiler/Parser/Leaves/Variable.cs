namespace NesDevCompiler.Parser;

public abstract class Variable : ILeafNode
{
	public string Instruction { get; }

	public Variable(string memoryPos)
	{
		Instruction = memoryPos;
	}
}