namespace Mossy.Parser.AbstractSyntaxTree;

public class VariableAssignent : Node, IStatement
{
	public string Identifier;
	public Expression Expression;

	public int Address;
	public Expression Offset;
	public bool IsGlobal;

	public override List<Node> GetChildren()
	{
		List<Node> children = [Expression];
		return children;
	}

	public Node AsNode()
	{
		return this;
	}

	public VariableAssignent(Node parent, string identifier, Expression expression) : base(parent)
	{
		Identifier = identifier;
		Expression = expression;
		Expression.parent = this;
	}
}