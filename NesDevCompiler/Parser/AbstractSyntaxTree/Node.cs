namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public abstract class Node
{
	public List<Node> children = new List<Node>();
	public Node parent;
	
	public bool IsRoot => parent == null;

	public Node(Node parent)
	{
		this.parent = parent;
	}
}