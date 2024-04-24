namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class CompileError : Node
{
	public readonly string Error;

	public CompileError(Node parent, string error) : base(parent)
	{
		Error = error;
	}
}