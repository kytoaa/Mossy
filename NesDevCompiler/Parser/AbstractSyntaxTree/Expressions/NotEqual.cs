namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class NotEqual : Expression
{
	public Expression Left;
	public Expression Right;


	public NotEqual(Node parent, Expression l, Expression r) : base(parent, "bool")
	{
		Left = l;
		Right = r;
	}
}