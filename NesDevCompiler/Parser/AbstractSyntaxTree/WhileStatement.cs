namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class WhileStatement : Node, IStatement
{
	public Expression Condition;
	public List<IStatement> Body = new List<IStatement>();

	public override List<Node> GetChildren()
	{
		List<Node> children = [Condition, .. Body.Select(s => s.AsNode())];
		return children;
	}

	public Node AsNode()
	{
		return this;
	}

	public WhileStatement(Node parent, Expression condition) : base(parent)
	{
		Condition = condition;
		Condition.parent = this;
	}
}