namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class VariableDeclaration : Node
{
	public string Identifier;
	public string Type;

	public int Size;
	public bool IsArray;

	public VariableAssignent? Assignent;

	public VariableDeclaration(Node parent, string identifier, string type, bool isArray = false, int size = 1, VariableAssignent? assignent = null) : base(parent)
	{
		Identifier = identifier;
		Type = type;
		IsArray = isArray;
		Size = size;
		Assignent = assignent;
	}
}