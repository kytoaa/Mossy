namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class WhileStatement : Statement
{
	public Expression Condition;
	public List<Statement> Body = new List<Statement>();

	public WhileStatement(Node parent, Expression condition) : base(parent)
	{
		Condition = condition;
	}
}