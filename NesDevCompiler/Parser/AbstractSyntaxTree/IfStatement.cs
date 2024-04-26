namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class IfStatement : Statement
{
	public List<Statement> body = new List<Statement>();

	public IfStatement(Node parent, Expression condition) : base(parent)
	{
	}
}