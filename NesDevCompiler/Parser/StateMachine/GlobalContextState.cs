using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public class GlobalContextState : State
{
	public override State Parse(ILexer lexer, Tree tree)
	{
		Token token = lexer.Next();
		if (token.Type != TokenType.Keyword)
			return new ErrorState($"Syntax Error: {token.Value} is not a valid keyword!");
		
		if (token.Value == "var")
		{
			//tree.current.children.Add(new DeclaredVariable())
		}
		
			
		return new ErrorState($"Syntax Error: {token.Value} is not a valid keyword! Only functions and declarations valid in this context!");
	}
}