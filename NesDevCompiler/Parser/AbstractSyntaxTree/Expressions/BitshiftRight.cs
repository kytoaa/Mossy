namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class BitshiftRight : Expression
{
	public Expression Value;


	public BitshiftRight(Node parent, Expression value) : base(parent, "int")
	{
		Value = value;
	}
}