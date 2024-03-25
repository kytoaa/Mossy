namespace NesDevCompiler.Parser;

public class Block : INode
{
	public INode[] Nodes { get; set; }

	public Block(INode[] nodes)
	{
		Nodes = nodes;
	}
}