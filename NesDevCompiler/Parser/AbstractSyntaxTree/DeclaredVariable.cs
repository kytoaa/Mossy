namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class DeclaredVariable : Variable
{
	public string Identifier;
	public int Size;
	public bool IsArray;

	public DeclaredVariable(Node parent, string identifier, bool isArray = false, int size = 1) : base(parent)
	{
		Identifier = identifier;
		IsArray = isArray;
		Size = size;
	}
}