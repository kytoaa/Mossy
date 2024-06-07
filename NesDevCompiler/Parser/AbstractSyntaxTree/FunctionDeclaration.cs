namespace Mossy.Parser.AbstractSyntaxTree;

public class FunctionDeclaration : Node
{
	public string Identifier;
	public string Type;
	public List<VariableDeclaration> Arguments;
	public Context Body;
	public int Size;

	public override List<Node> GetChildren()
	{
		List<Node> children = [Body, .. Arguments];
		return children;
	}

	public FunctionDeclaration(Node parent, string type) : base(parent)
	{
		Type = type;
	}
}