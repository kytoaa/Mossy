using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public interface IParser
{
	public Node Parse(ILexer lexer);
}