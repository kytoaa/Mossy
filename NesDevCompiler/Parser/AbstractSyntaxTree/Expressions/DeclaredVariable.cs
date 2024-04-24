namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class DeclaredVariable : Expression
{
	public string Identifier;


	public DeclaredVariable(Node parent, string type, string identifier) : base(parent, type)
	{
		Identifier = identifier;
	}
}