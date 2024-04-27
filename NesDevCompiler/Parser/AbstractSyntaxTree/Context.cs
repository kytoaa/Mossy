namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class Context : Node
{
	public List<VariableDeclaration> variables = new List<VariableDeclaration>();
	public List<FunctionDeclaration> functions = new List<FunctionDeclaration>();

	public List<IStatement> statements = new List<IStatement>();

	public override List<Node> GetChildren()
	{
		List<Node> children = [..variables, ..functions, ..statements.Select(s => s.AsNode())];
		return children;
	}

	public Context() : base(null)
	{
		
	}
}