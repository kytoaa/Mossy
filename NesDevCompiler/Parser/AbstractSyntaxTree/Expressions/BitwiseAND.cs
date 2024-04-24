namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class BitwiseAND : Expression
{
	public Expression Left;
	public Expression Right;


	public BitwiseAND(Node parent, Expression l, Expression r) : base(parent, "int")
	{
		Left = l;
		Right = r;
	}
}