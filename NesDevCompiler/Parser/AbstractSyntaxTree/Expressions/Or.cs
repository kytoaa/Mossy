namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Or : Expression
{
	public Expression Left;
	public Expression Right;


	public Or(Node parent, Expression l, Expression r) : base(parent, "bool")
	{
		Left = l;
		Right = r;
	}
}