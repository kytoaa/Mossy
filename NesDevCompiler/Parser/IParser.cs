using Mossy.Lexer;
using Mossy.Parser.AbstractSyntaxTree;

namespace Mossy.Parser;

public interface IParser
{
	public Node Parse(ILexer lexer);
}