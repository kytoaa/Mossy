namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class FunctionCall : Statement
{
	public List<Expression> Arguments = new List<Expression>();
	public string Identifier;

	public FunctionCall(Node parent, string identifier) : base(parent)
	{
		Identifier = identifier;
	}
}