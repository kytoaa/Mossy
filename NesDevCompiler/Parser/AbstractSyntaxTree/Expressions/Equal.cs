namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Equal : Expression
{
	public Expression Left;
	public Expression Right;


	public Equal(Node parent, Expression l, Expression r) : base(parent, "bool")
	{
		Left = l;
		Right = r;
	}
}