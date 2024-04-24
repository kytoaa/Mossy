namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class BitshiftLeft : Expression
{
	public Expression Value;


	public BitshiftLeft(Node parent, Expression value) : base(parent, "int")
	{
		Value = value;
	}
}