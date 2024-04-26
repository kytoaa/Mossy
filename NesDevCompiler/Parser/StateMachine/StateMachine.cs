using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NesDevCompiler.Parser;

public class StateMachine
{
	public Node Parse(ILexer lexer, Tree tree)
	{
		return GlobalContextState(lexer);
	}

	private Context GlobalContextState(ILexer lexer)
	{
		Context globalContext = new Context();

		while (!lexer.End())
		{
			Token token = lexer.Next();
			if (token.Type != TokenType.Keyword)
				throw new CompileError($"Syntax Error: {token.Value} is not a valid keyword!");

			if (token.Value == "var")
			{
				globalContext.variables.Add(VarState(lexer, globalContext));
			}
			else if (token.Value == "func")
			{
				globalContext.functions.Add(FunctionState(lexer, globalContext));
			}
			//else
				//throw new CompileError($"Syntax Error: {token.Value} is not a valid keyword! Only functions and declarations valid in this context!");
		}
		return globalContext;


	}

	private Context FunctionContextState(ILexer lexer, Node parent)
	{
		Context context = new Context() { parent = parent };
		while (lexer.Peek().Value != "}")
		{
			Token next = lexer.Next();
			if ((next.Type != TokenType.Identifier) && (next.Type != TokenType.Keyword))
				throw new CompileError($"Syntax Error: {next.Value} is not a valid statement!");

			if (next.Value == "var")
			{
				context.variables.Add(VarState(lexer, context));
			}
			if (next.Value == "if")
			{
				context.statements.Add(IfStatementState(lexer, context));
			}
			if (next.Value == "while")
			{
				context.statements.Add(WhileStatementState(lexer, context));
			}
			if (next.Type == TokenType.Identifier)
			{
				context.statements.Add(StatementState(lexer, context, next));
			}
		}
		Token closeCurly = lexer.Next();
		if (closeCurly.Value != "}")
			throw new CompileError($"Syntax Error: function body not closed");
		return context;
	}

	private Statement StatementState(ILexer lexer, Node parent, Token next)
	{
		Token funcOrAssign = lexer.Next();
		if ((funcOrAssign.Value != "(") && (funcOrAssign.Value != "="))
			throw new CompileError($"Syntax Error: {next.Value} {funcOrAssign.Value} is not a valid statement, only assignments and function calls are allowed!");

		if (funcOrAssign.Value == "(")
		{
			List<Expression> args = new List<Expression>();
			while (lexer.Peek().Value != ")")
			{
				if (lexer.Peek().Value == ";")
					break;
				args.Add(ParseExpression(lexer));
			}
			Token closeBrackets = lexer.Next();
			if (closeBrackets.Value != ")")
				throw new CompileError($"Syntax Error: argument list must be closed!");

			Token endStatement = lexer.Next();
			if (endStatement.Value != ";")
				throw new CompileError($"Syntax Error: function declaration never ends, you forgot a semicolon!");
			Statement statement = new FunctionCall(parent, next.Value) { Arguments = args };
			return statement;
		}
		else if (funcOrAssign.Value == "=")
		{
			Statement statement = new VariableAssignent(parent, next.Value, ParseExpression(lexer));
			Token endStatement = lexer.Next();
			if (endStatement.Value != ";")
				throw new CompileError($"Syntax Error: function declaration never ends, you forgot a semicolon!");
			return statement;
		}
		else
			throw new CompileError($"Syntax Error: not a valid statement, only assignments and function calls are allowed!");
	}

	private Statement IfStatementState(ILexer lexer, Context parent)
	{
		Token openBrackets = lexer.Next();
		if (openBrackets.Value != "(")
			throw new CompileError($"Syntax Error: if statements must have a condition");
		Expression condition = ParseExpression(lexer);
		Token closeBrackets = lexer.Next();
		if (closeBrackets.Value != ")")
			throw new CompileError($"Syntax Error: if statement condition must be closed");

		IfStatement ifStatement = new IfStatement(parent, condition);

		Token openCurly = lexer.Next();
		if (openCurly.Value != "{")
			throw new CompileError($"Syntax Error: if statement must have a body");

		Context context = FunctionContextState(lexer, ifStatement);

		ifStatement.Body = context.statements;

		if (lexer.Peek().Value != "else")
			return ifStatement;

		lexer.Next();
		Token elseOpenCurly = lexer.Next();
		if (elseOpenCurly.Value != "{")
			throw new CompileError($"Syntax Error: if statement must have a body");

		Context elseContext = FunctionContextState(lexer, ifStatement);

		ifStatement.ElseBody = elseContext.statements;

		return ifStatement;
	}
	private Statement WhileStatementState(ILexer lexer, Context parent)
	{
		Token openBrackets = lexer.Next();
		if (openBrackets.Value != "(")
			throw new CompileError($"Syntax Error: if statements must have a condition");
		Expression condition = ParseExpression(lexer);
		Token closeBrackets = lexer.Next();
		if (closeBrackets.Value != ")")
			throw new CompileError($"Syntax Error: if statement condition must be closed");

		WhileStatement whileStatement = new WhileStatement(parent, condition);

		Token openCurly = lexer.Next();
		if (openCurly.Value != "{")
			throw new CompileError($"Syntax Error: if statement must have a body");

		Context context = FunctionContextState(lexer, whileStatement);

		whileStatement.Body = context.statements;
		return whileStatement;
	}

	private VariableDeclaration VarState(ILexer lexer, Node parent)
	{
		Token varType = lexer.Next();
		if (varType.Value != "bool" && varType.Value != "int")
			throw new CompileError($"Syntax Error: {varType.Value} is not a valid type!");

		bool isArray = lexer.Peek().Value == "[";
		int length = 1;
		if (isArray)
		{
			lexer.Next();
			Token arrayLength = lexer.Next();
			if (!int.TryParse(arrayLength.Value, out length))
				throw new CompileError($"Syntax Error: {arrayLength.Value} is not a valid integer length!");
			if (length < 1)
				throw new CompileError($"Syntax Error: {arrayLength.Value} is not a valid array length!");

			Token arrayClose = lexer.Next();
			if (arrayClose.Value != "]")
				throw new CompileError($"Syntax Error: length of array only accepts one argument!");
		}

		Token varIdentifier = lexer.Next();
		if (varIdentifier.Type != TokenType.Identifier)
			throw new CompileError($"Syntax Error: {varIdentifier.Value} is not a valid identifier!");
		
		Token end = lexer.Next();

		if (end.Value == ";")
		{
			VariableDeclaration variable = new VariableDeclaration(parent, varIdentifier.Value, varType.Value, isArray, length);
			return variable;
		}
		else if (end.Value == "=")
		{
			if (isArray)
				throw new CompileError($"Syntax Error: arrays cannot be initialized with expressions!");
			VariableAssignent assignment = new VariableAssignent(null, varIdentifier.Value, ParseExpression(lexer));
			VariableDeclaration declaration = new VariableDeclaration(parent, varIdentifier.Value, varType.Value, isArray, length, assignment);
			Token semicolon = lexer.Next();
			if (semicolon.Value != ";")
				throw new CompileError($"Syntax Error: variable declaration not closed, did you forget a semicolon!");
			return declaration;
		}

		throw new CompileError($"Syntax Error: not a valid variable declaration!");
	}

	private FunctionDeclaration FunctionState(ILexer lexer, Context context)
	{
		Token functionType = lexer.Next();
		if (functionType.Value != "bool" && functionType.Value != "int")
			throw new CompileError($"Syntax Error: {functionType.Value} is not a valid type!");

		Token functionIdentifier = lexer.Next();
		if (functionIdentifier.Type != TokenType.Identifier)
			throw new CompileError($"Syntax Error: {functionIdentifier.Value} is not a valid identifier!");

		Token openBrackets = lexer.Next();
		if (openBrackets.Value != "(")
			throw new CompileError($"Syntax Error: function declaration must open with an optional argument list!");

		FunctionDeclaration functionDeclaration = new FunctionDeclaration(context);

		List<VariableDeclaration> arguments = new List<VariableDeclaration>();
		while (lexer.Peek().Value != ")")
		{
			arguments.Add(VarState(lexer, functionDeclaration));
		}
		lexer.Next();
		Token openCurly = lexer.Next();
		if (openCurly.Value != "{")
			throw new CompileError($"Syntax Error: function declaration must have a body!");

		Context functionContext = FunctionContextState(lexer, functionDeclaration);

		functionDeclaration.Arguments = arguments;
		functionDeclaration.Body = functionContext;
		functionDeclaration.Identifier = functionIdentifier.Value;
		functionDeclaration.Size = functionContext.variables.Count;

		return functionDeclaration;
	}

	private Expression ParseExpression(ILexer lexer)
	{
		Expression expression = new ExpressionParser().Parse(lexer);
		//expression.parent = tree.current;

		return expression;
	}

	private FunctionCall ParseFunction(ILexer lexer, string identifier)
	{
		throw new NotImplementedException();
	}


/*	private Tree ThrowError(string error, Tree tree)
	{
		tree.current = new CompileError(tree.current, error);
		return tree;
	}*/
}
