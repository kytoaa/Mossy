namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class DeclaredVariable : Expression
{
	public string Identifier;
	public int Offset;

	public DeclaredVariable(Node parent, string type, string identifier, int offset = 0) : base(parent, type)
	{
		Identifier = identifier;
		Offset = offset;
	}
}