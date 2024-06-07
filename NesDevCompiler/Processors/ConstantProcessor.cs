using System;
using Mossy.Lexer;

namespace Mossy.Processors;

public class ConstantProcessor : ILexerProcessor
{
	private Dictionary<string, string> _constants = new Dictionary<string, string>();

	public Func<ILexer, Token> Process(Token input)
	{
		if (input.Value == "const")
		{
			return lexer =>
			{
				lexer.Next(); // Skip the type.
				string identifier = lexer.Next().Value;
				if (lexer.Next().Value != "=")
					throw new Parser.AbstractSyntaxTree.CompileError($"Syntax Error: constant {identifier} must have an initial value!");
				string value = lexer.Next().Value;
				_constants.Add(identifier, value);
				if (lexer.Next().Value != ";")
					throw new Parser.AbstractSyntaxTree.CompileError($"Syntax Error: constant declaration {identifier} must be closed!");
				return lexer.Next();
			};
		}
		if (_constants.Keys.Contains(input.Value))
		{
			return lexer => new Token(TokenType.Value, _constants[input.Value]);
		}
		return lexer => input;

		throw new NotImplementedException();
	}
}