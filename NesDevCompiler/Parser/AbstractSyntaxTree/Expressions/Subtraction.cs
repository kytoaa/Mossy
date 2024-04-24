namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Subtraction : Expression
{
	public Expression Left;
	public Expression Right;


	public Subtraction(Node parent, Expression l, Expression r) : base(parent, "int")
	{
		Left = l;
		Right = r;
	}
}