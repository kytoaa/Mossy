namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public abstract class Variable : Node
{
	public string Type;

	protected Variable(Node parent) : base(parent)
	{
	}
}