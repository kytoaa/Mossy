namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class GlobalContext : Node
{
	public List<VariableDeclaration> variables;
	public List<FunctionDeclaration> functions;

	public GlobalContext() : base(null)
	{
		
	}
}