namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class And : Expression
{
	public Expression Left;
	public Expression Right;


	public And(Node parent, Expression l, Expression r) : base(parent, "bool")
	{
		Left = l;
		Right = r;
	}
}