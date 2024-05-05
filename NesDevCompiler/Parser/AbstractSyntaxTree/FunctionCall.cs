namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class FunctionCall : Expression, IStatement
{
	public List<Expression> Arguments = new List<Expression>();
	public string Identifier;
	public int Size;

	public override List<Node> GetChildren()
	{
		List<Node> children = new List<Node>(Arguments);
		return children;
	}

	public Node AsNode()
	{
		return this;
	}

	public FunctionCall(Node parent, string identifier, List<Expression>? arguments = null) : base(parent, "default")
	{
		Identifier = identifier;
		if (arguments != null)
		{
			foreach (Expression expression in arguments)
			{
				expression.parent = this;
			}
			Arguments = arguments;
		}
	}
}