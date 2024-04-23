namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class ConstantValue : Variable
{
	public string Value;

	public ConstantValue(Node parent, string value) : base(parent)
	{
		Value = value;
	}
}