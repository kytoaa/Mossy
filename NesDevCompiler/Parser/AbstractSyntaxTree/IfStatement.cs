using System.Reflection.Metadata.Ecma335;

namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class IfStatement : Node, IStatement
{
	public Expression Condition;
	public List<IStatement> Body = new List<IStatement>();
	public List<IStatement>? ElseBody = null;

	public override List<Node> GetChildren()
	{
		List<Node> children = [Condition, .. Body.Select(s => s.AsNode())];
		if (ElseBody != null)
		{
			foreach (Node node in ElseBody)
			{
				children.Add(node);
			}
		}
		return children;
	}

	public Node AsNode()
	{
		return this;
	}

	public IfStatement(Node parent, Expression condition) : base(parent)
	{
		Condition = condition;
		Condition.parent = this;
	}
}