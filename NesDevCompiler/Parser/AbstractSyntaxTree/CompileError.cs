namespace Mossy.Parser.AbstractSyntaxTree;

public class CompileError : Exception
{
	public CompileError(string? message) : base(message)
	{

	}
}