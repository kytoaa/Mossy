using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public class StateMachine
{
	public void Parse(ILexer lexer, Tree tree)
	{
		
	}

	private Tree GlobalContextState(ILexer lexer, Tree tree)
	{
		Token token = lexer.Next();
		if (token.Type != TokenType.Keyword)
			return ThrowError($"Syntax Error: {token.Value} is not a valid keyword!", tree);

		if (token.Value == "var")
		{
			// TODO var declaration stuff idk
		}
		if (token.Value == "func")
		{
			// TODO func declaration
		}


		return ThrowError($"Syntax Error: {token.Value} is not a valid keyword! Only functions and declarations valid in this context!", tree);
	}

	private Tree VarState(ILexer lexer, Tree tree)
	{
		Token varType = lexer.Next();
		if (varType.Value != "bool" && varType.Value != "int")
			return ThrowError($"Syntax Error: {varType.Value} is not a valid type!", tree);

		bool isArray = lexer.Peek().Value == "[";
		int length = 1;
		if (isArray)
		{
			lexer.Next();
			Token arrayLength = lexer.Next();
			if (!int.TryParse(arrayLength.Value, out length))
				return ThrowError($"Syntax Error: {arrayLength.Value} is not a valid integer length!", tree);
			if (length < 1)
				return ThrowError($"Syntax Error: {arrayLength.Value} is not a valid array length!", tree);

			Token arrayClose = lexer.Next();
			if (arrayClose.Value != "]")
				return ThrowError($"Syntax Error: length of array only accepts one argument!", tree);
		}

		Token varIdentifier = lexer.Next();
		if (varIdentifier.Type != TokenType.Identifier)
			return ThrowError($"Syntax Error: {varIdentifier.Value} is not a valid identifier!", tree);

		if (lexer.Peek().Value == ";")
		{
			VariableDeclaration variable = new VariableDeclaration(tree.current, varIdentifier.Value, varType.Value, isArray, length);
			((Context)tree.current).variables.Add(variable);
			return tree;
		}
		else if (lexer.Peek().Value == "=")
		{

		}

		return ThrowError($"Syntax Error: length of array only accepts one argument!", tree);
	}

	private Expression ParseExpression(ILexer lexer, Tree tree)
	{
		Token next = lexer.Next();
		if (next.Type == TokenType.Identifier)
		{
			if (lexer.Peek().Value == "(")
			{
				return ParseFunction(lexer, next.Value);
			}
			return new DeclaredVariable(tree.current, "default", next.Value);
		}

		throw new NotImplementedException();
	}

	private FunctionCall ParseFunction(ILexer lexer, string identifier)
	{
		throw new NotImplementedException();
	}


	private Tree ThrowError(string error, Tree tree)
	{
		tree.current = new CompileError(tree.current, error);
		return tree;
	}
}
