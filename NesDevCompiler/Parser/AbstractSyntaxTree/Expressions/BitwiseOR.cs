namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class BitwiseOR : Expression
{
	public Expression Left;
	public Expression Right;


	public BitwiseOR(Node parent, Expression l, Expression r) : base(parent, "int")
	{
		Left = l;
		Right = r;
	}
}