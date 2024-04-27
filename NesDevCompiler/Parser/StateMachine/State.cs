using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public abstract class State
{
	public abstract State Parse(ILexer lexer, Tree tree);

}