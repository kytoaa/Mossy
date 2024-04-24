namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Not : Expression
{
	public Expression Value;

	public Not(Node parent, Expression value) : base(parent, "bool")
	{
		Value = value;
	}
}