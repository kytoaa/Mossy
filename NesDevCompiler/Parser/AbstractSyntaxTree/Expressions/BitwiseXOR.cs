namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class BitwiseXOR : Expression
{
	public Expression Left;
	public Expression Right;


	public BitwiseXOR(Node parent, Expression l, Expression r) : base(parent, "int")
	{
		Left = l;
		Right = r;
	}
}