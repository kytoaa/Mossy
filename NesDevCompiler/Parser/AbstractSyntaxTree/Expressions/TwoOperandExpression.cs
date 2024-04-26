namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class TwoOperandExpression : Expression
{
	public Expression Left;
	public Expression Right;
	public string Operator;


	public TwoOperandExpression(Node parent, string type, Expression l, Expression r, string op) : base(parent, type)
	{
		Left = l;
		Right = r;
		Operator = op;
		l.parent = this;
		r.parent = this;
	}
}