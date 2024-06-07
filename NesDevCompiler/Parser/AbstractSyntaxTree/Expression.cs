namespace Mossy.Parser.AbstractSyntaxTree;

public abstract class Expression : Node
{
	public string Type;

	protected Expression(Node parent, string type) : base(parent)
	{
		Type = type;
	}

	public Expression SetParent(Node parent)
	{
		this.parent = parent;
		return this;
	}
}