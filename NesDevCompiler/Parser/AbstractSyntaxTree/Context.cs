namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Context : Node
{
	public List<VariableDeclaration> variables = new List<VariableDeclaration>();
	public List<FunctionDeclaration> functions = new List<FunctionDeclaration>();

	public List<Statement> statements = new List<Statement>();

	public Context() : base(null)
	{
		
	}
}