namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class FunctionCall : Statement
{
	public List<Expression> Arguments = new List<Expression>();

	public FunctionCall(Node parent) : base(parent)
	{
	}
}