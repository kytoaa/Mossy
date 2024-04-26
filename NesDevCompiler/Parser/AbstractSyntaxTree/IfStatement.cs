namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class IfStatement : Statement
{
	public Expression Condition;
	public List<Statement> Body = new List<Statement>();
	public List<Statement>? ElseBody = null;

	public IfStatement(Node parent, Expression condition) : base(parent)
	{
		Condition = condition;
	}
}