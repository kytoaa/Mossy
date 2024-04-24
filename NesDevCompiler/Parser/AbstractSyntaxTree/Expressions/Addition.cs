namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Addition : Expression
{
	public Expression Left;
	public Expression Right;


	public Addition(Node parent, Expression l, Expression r) : base(parent, "int")
	{
		Left = l;
		Right = r;
	}
}