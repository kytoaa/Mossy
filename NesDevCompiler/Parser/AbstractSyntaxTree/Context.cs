namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Context : INode
{
	public int byteCount = 0;
	public List<INode> children;

	public Context(List<INode> children)
	{
		this.children = children;
		
	}
}