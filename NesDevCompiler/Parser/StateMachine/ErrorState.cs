using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public class ErrorState : State
{
	public readonly string Error;

	public ErrorState(string error)
	{
		Error = error;
	}

	public override State Parse(ILexer lexer, Tree tree)
	{
		if (tree.current is CompileError)
			return this;
		//tree.current.children.Add(new CompileError(tree.current, Error));
		return this;
	}
}