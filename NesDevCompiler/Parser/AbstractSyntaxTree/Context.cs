namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Context : Node
{
	public List<VariableDeclaration> variables = new List<VariableDeclaration>();
	public List<FunctionDeclaration> functions = new List<FunctionDeclaration>();

	public Context() : base(null)
	{
		
	}
}