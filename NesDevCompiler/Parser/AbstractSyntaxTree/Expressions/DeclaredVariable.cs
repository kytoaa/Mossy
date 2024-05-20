namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class DeclaredVariable : Expression
{
	public string Identifier;
	public Expression? Offset;
	public int Address;
	public bool IsGlobal;

	public override List<Node> GetChildren()
	{
		List<Node> children = new List<Node>();
		return children;
	}

	public DeclaredVariable(Node parent, string type, string identifier, Expression? offset = null) : base(parent, type)
	{
		Identifier = identifier;
		Offset = offset;
	}
}