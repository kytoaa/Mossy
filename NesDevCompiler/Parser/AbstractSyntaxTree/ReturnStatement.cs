using System.Reflection.Metadata.Ecma335;

namespace Mossy.Parser.AbstractSyntaxTree;

public class ReturnStatement : Node, IStatement
{
	public Expression ReturnValue;

	public override List<Node> GetChildren()
	{
		return new List<Node>() { ReturnValue };
	}

	public Node AsNode()
	{
		return this;
	}

	public ReturnStatement(Node parent, Expression returnValue) : base(parent)
	{
		ReturnValue = returnValue;
		returnValue.parent = this;
	}
}