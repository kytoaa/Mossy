namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class VariableDeclaration : Statement
{
	public string Identifier;
	public string Type;

	public VariableDeclaration(Node parent, string identifier, string type) : base(parent)
	{
		Identifier = identifier;
		Type = type;
	}
}