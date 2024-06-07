using System.Reflection.Metadata.Ecma335;

namespace Mossy.Parser.AbstractSyntaxTree;

public class BreakStatement : Node, IStatement
{
	public override List<Node> GetChildren()
	{
		return new List<Node>();
	}

	public Node AsNode()
	{
		return this;
	}

	public BreakStatement(Node parent) : base(parent)
	{

	}
}