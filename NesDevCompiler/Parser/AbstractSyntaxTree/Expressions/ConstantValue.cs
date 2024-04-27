namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class ConstantValue : Expression
{
	public string Value;

	public override List<Node> GetChildren()
	{
		List<Node> children = new List<Node>();
		return children;
	}

	public ConstantValue(Node parent, string type, string value) : base(parent, type)
	{
		Value = value;
	}
}