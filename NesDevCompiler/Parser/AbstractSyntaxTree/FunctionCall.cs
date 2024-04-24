namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class FunctionCall : Expression
{
	public List<Expression> Arguments = new List<Expression>();

	public FunctionCall(Node parent, string type) : base(parent, type)
	{
	}
}