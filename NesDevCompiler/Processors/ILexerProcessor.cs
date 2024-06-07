using System;
using Mossy.Lexer;

namespace Mossy.Processors;

public interface ILexerProcessor
{
	public Func<ILexer, Token> Process(Token input);
}