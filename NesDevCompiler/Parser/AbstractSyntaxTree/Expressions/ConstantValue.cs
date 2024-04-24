namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class ConstantValue : Expression
{
	public string Value;

	public ConstantValue(Node parent, string type, string value) : base(parent, type)
	{
		Value = value;
	}
}