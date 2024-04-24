namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class VariableAssignent : Statement
{
	public string Identifier;
	public Expression Expression;

	public VariableAssignent(Node parent, string identifier, Expression expression) : base(parent)
	{
		Identifier = identifier;
		Expression = expression;
	}
}