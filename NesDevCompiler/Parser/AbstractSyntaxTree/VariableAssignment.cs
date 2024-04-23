namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class VariableAssignent : Statement
{
	public string Identifier;
	public Node Expression;

	public VariableAssignent(Node parent, string identifier) : base(parent)
	{
		Identifier = identifier;
	}
}