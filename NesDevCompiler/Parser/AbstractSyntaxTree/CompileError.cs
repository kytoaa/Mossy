namespace NesDevCompiler.Parser.AbstractSyntaxTree;

public class CompileError : Exception
{
	public CompileError(string? message) : base(message)
	{

	}
}