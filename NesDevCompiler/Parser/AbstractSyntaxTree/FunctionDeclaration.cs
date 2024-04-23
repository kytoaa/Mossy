namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class FunctionDeclaration : Node
{
	public string Identifier;
	public List<VariableDeclaration> Arguments;
	public Node Body;
	public int Size;

	public FunctionDeclaration(Node parent) : base(parent)
	{
	}
}