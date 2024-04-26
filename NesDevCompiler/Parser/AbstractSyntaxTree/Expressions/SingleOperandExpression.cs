namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class SingleOperandExpression : Expression
{
	public Expression Value;
	public string Operator;

	public SingleOperandExpression(Node parent, string type, Expression value, string op) : base(parent, type)
	{
		Value = value;
		Operator = op;
		value.parent = this;
	}
}