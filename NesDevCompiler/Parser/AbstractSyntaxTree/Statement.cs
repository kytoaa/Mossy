namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public abstract class Statement : Node
{
	protected Statement(Node parent) : base(parent)
	{
	}
}