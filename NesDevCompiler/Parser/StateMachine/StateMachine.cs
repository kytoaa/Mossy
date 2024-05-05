using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

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
			else
				throw new CompileError($"Syntax Error: {token.Value} is not a valid keyword! Only functions and declarations valid in this context!");
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
				VariableDeclaration varDecl = VarState(lexer, context);
				context.variables.Add(varDecl);
				if (varDecl.Assignent != null)
				{
					context.statements.Add(varDecl.Assignent);
				}
			}
			else if (next.Value == "if")
			{
				context.statements.Add(IfStatementState(lexer, context));
			}
			else if (next.Value == "while")
			{
				context.statements.Add(WhileStatementState(lexer, context));
			}
			else if (next.Value == "return")
			{
				ReturnStatement returnStatement = new ReturnStatement(context, ParseExpression(lexer));

				Token semicolon = lexer.Next();
				if (semicolon.Value != ";")
					throw new CompileError("Syntax Error: return statement not closed, did you forget a semicolon!");

				context.statements.Add(returnStatement);
			}
			else if (next.Type == TokenType.Identifier)
			{
				context.statements.Add(StatementState(lexer, context, next));
			}
			else
				throw new CompileError($"Syntax Error: {next.Value} is not a valid statement!");
		}
		Token closeCurly = lexer.Next();
		if (closeCurly.Value != "}")
			throw new CompileError($"Syntax Error: function body not closed!");
		return context;
	}

	private IStatement StatementState(ILexer lexer, Node parent, Token next)
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
				throw new CompileError($"Syntax Error: function declaration {next.Value} never ends, you forgot a semicolon!");
			IStatement statement = new FunctionCall(parent, next.Value, args);// { Arguments = args };
			return statement;
		}
		else if (funcOrAssign.Value == "=")
		{
			IStatement statement = new VariableAssignent(parent, next.Value, ParseExpression(lexer));
			Token endStatement = lexer.Next();
			if (endStatement.Value != ";")
				throw new CompileError($"Syntax Error: variable declaration {next.Value} never ends, you forgot a semicolon!");
			return statement;
		}
		else
			throw new CompileError($"Syntax Error: not a valid statement, only assignments and function calls are allowed!");
	}

	private IStatement IfStatementState(ILexer lexer, Context parent)
	{
		Token openBrackets = lexer.Next();
		if (openBrackets.Value != "(")
			throw new CompileError($"Syntax Error: if statements must have a condition!");
		Expression condition = ParseExpression(lexer);
		Token closeBrackets = lexer.Next();
		if (closeBrackets.Value != ")")
			throw new CompileError($"Syntax Error: if statement condition must be closed!");

		IfStatement ifStatement = new IfStatement(parent, condition);

		Token openCurly = lexer.Next();
		if (openCurly.Value != "{")
			throw new CompileError($"Syntax Error: if statement must have a body!");

		Context context = FunctionContextState(lexer, ifStatement);

		ifStatement.Body = context.statements;

		if (context.variables.Count > 0)
			throw new CompileError("Syntax Error: local variables may not be declared within if statements!");
		if (context.functions.Count > 0)
			throw new CompileError("Syntax Error: functions may not be declared within if statements!");

		if (lexer.Peek().Value != "else")
			return ifStatement;

		lexer.Next();
		Token elseOpenCurly = lexer.Next();
		if (elseOpenCurly.Value != "{")
			throw new CompileError($"Syntax Error: if statement must have a body!");

		Context elseContext = FunctionContextState(lexer, ifStatement);

		ifStatement.ElseBody = elseContext.statements;

		return ifStatement;
	}
	private IStatement WhileStatementState(ILexer lexer, Context parent)
	{
		Token openBrackets = lexer.Next();
		if (openBrackets.Value != "(")
			throw new CompileError($"Syntax Error: while statements must have a condition!");
		Expression condition = ParseExpression(lexer);
		Token closeBrackets = lexer.Next();
		if (closeBrackets.Value != ")")
			throw new CompileError($"Syntax Error: while statement condition must be closed!");

		WhileStatement whileStatement = new WhileStatement(parent, condition);

		Token openCurly = lexer.Next();
		if (openCurly.Value != "{")
			throw new CompileError($"Syntax Error: while statement must have a body!");

		Context context = FunctionContextState(lexer, whileStatement);

		whileStatement.Body = context.statements;

		if (context.variables.Count > 0)
			throw new CompileError("Syntax Error: local variables may not be declared within while loops!");
		if (context.functions.Count > 0)
			throw new CompileError("Syntax Error: functions may not be declared within while loops!");

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

		FunctionDeclaration functionDeclaration = new FunctionDeclaration(context, functionType.Value);

		List<VariableDeclaration> arguments = new List<VariableDeclaration>();
		while (lexer.Peek().Value != ")")
		{
			VariableDeclaration var = VarState(lexer, functionDeclaration);
			if (var.Assignent != null)
				throw new CompileError($"Syntax Error: function arguments cannot have default values!");
			arguments.Add(var);
		}
		lexer.Next();
		Token openCurly = lexer.Next();
		if (openCurly.Value != "{")
			throw new CompileError($"Syntax Error: {openCurly} is not a valid function body!");

		Context functionContext = FunctionContextState(lexer, functionDeclaration);


		functionContext.variables = arguments.Union(functionContext.variables).ToList();

		functionDeclaration.Arguments = arguments;
		functionDeclaration.Body = functionContext;
		functionDeclaration.Identifier = functionIdentifier.Value;
		functionDeclaration.Size = functionContext.variables.Count;

		return functionDeclaration;
	}

	private Expression ParseExpression(ILexer lexer)
	{
		Token next = lexer.Peek();
		if (!((next.Type == TokenType.Identifier) || (next.Type == TokenType.Value) || (next.Value == "(") || (next.Type == TokenType.Operator)))
			throw new CompileError($"Syntax Error: {next.Value} is not a valid expression!");

		if ((next.Type == TokenType.Identifier) && (lexer.Peek(true, 1).Value == "("))
		{
			lexer.Next();
			lexer.Next();
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

			Expression statement = new FunctionCall(null, next.Value, args);// { Arguments = args };
			return statement;
		}
		else
		{
			Expression expression = new ExpressionParser().Parse(lexer);
			return expression;
		}
		//expression.parent = parent;

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
