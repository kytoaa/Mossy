namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class VariableDeclaration : Node
{
	public string Identifier;
	public string Type;

	public int Size;
	public bool IsArray;

	public int Address;
	public bool IsGlobal;
	public bool IsArgument = false;

	public VariableAssignent? Assignent;

	public override List<Node> GetChildren()
	{
		List<Node> children = new List<Node>();
		if (Assignent != null)
		{
			children.Add(Assignent);
		}
		return children;
	}

	public VariableDeclaration(Node parent, string identifier, string type, bool isArray = false, int size = 1, VariableAssignent? assignent = null) : base(parent)
	{
		Identifier = identifier;
		Type = type;
		IsArray = isArray;
		Size = size;
		Assignent = assignent;
		if (assignent != null)
			assignent.parent = this;
	}
}