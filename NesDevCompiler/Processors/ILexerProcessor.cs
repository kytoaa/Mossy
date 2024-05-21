using System;
using NesDevCompiler.Lexer;

namespace NesDevCompiler.Processors;

public interface ILexerProcessor
{
	public Func<ILexer, Token> Process(Token input);
}