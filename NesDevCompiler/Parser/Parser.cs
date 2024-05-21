using NesDevCompiler.Lexer;
using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.Parser;

public class Parser : IParser
{
	public Node Parse(ILexer lexer)
	{
		Warnings.ClearWarnings();
		return ParseAST(lexer);
	}

	private Node ParseAST(ILexer lexer)
	{
		Node root = new Context();
		Tree tree = new Tree(root);

		StateMachine stateMachine = new StateMachine();
		Node ast = stateMachine.Parse(lexer);

		SetParent(ast);
		CheckVariablesAndFunctions(ast);

		return ast;
	}

	private record struct Variable(string identifier, string type, int address, bool isGlobal);
	private record struct Function(string identifier, string type, List<string> args, int size);

	private void CheckVariablesAndFunctions(Node node, List<Variable> variables = null, List<Function> functions = null)
	{
		bool isGlobal = (variables == null) && (functions == null);
		if (variables == null)
			variables = new List<Variable>();
		if (functions == null)
			functions = new List<Function>();

		if (node is Context)
		{
			int varIndex = 0;
			foreach (VariableDeclaration variableDeclaration in ((Context)node).variables)
			{
				variables.Add(new Variable(variableDeclaration.Identifier, variableDeclaration.Type, varIndex, isGlobal));
				variableDeclaration.Address = varIndex;
				variableDeclaration.IsGlobal = isGlobal;
				if (variableDeclaration.Assignent != null)
				{
					variableDeclaration.Assignent.Address = varIndex;
					variableDeclaration.Assignent.IsGlobal = isGlobal;
				}
				else if (!variableDeclaration.IsArgument)
				{
					Warnings.AddWarning($"Variable Declaration {variableDeclaration.Identifier} does not have an initial assignment, it may contain an unknown value!");
				}
				varIndex += 1;
			}
			foreach (FunctionDeclaration functionDeclaration in ((Context)node).functions)
			{
				functions.Add(new Function(functionDeclaration.Identifier, functionDeclaration.Type, functionDeclaration.Arguments.Select(f => f.Type).ToList(), functionDeclaration.Size));
			}
		}

		foreach (Node child in node.GetChildren())
		{
			CheckVariablesAndFunctions(child, new List<Variable>(variables), new List<Function>(functions));
		}
		if (node is Expression)
		{
			((Expression)node).Type = ResolveType((Expression)node);
		}

		if (node is DeclaredVariable)
		{
			Variable varDecl = variables.Find(v => v.identifier == ((DeclaredVariable)node).Identifier);
			if (varDecl == default)
				throw new CompileError($"Syntax Error: {((DeclaredVariable)node).Identifier} does not exist within this context!");

			((DeclaredVariable)node).Type = varDecl.type;
			((DeclaredVariable)node).Address = varDecl.address;
			((DeclaredVariable)node).IsGlobal = varDecl.isGlobal;
		}
		if (node is VariableAssignent)
		{
			VariableAssignent variableAssignent = (VariableAssignent)node;
			Variable varDecl = variables.Find(v => v.identifier == ((VariableAssignent)node).Identifier);
			if (varDecl == default)
				throw new CompileError($"Syntax Error: {((VariableAssignent)node).Identifier} does not exist within this context!");

			variableAssignent.IsGlobal = varDecl.isGlobal;
			variableAssignent.Address = varDecl.address;

			if (variableAssignent.Expression is FunctionCall)
			{
				Function funcCall = functions.Find(f => f.identifier == ((FunctionCall)variableAssignent.Expression).Identifier);
				if (funcCall == default)
					throw new CompileError($"Syntax Error: {((FunctionCall)node).Identifier} does not exist within this context!");
				variableAssignent.Expression.Type = funcCall.type;
			}

			if (variableAssignent.Expression.Type != varDecl.type)
				throw new CompileError($"Type Error: {variableAssignent.Identifier} assignment {variableAssignent.Expression.Type} is not of type {varDecl.type}");
		}
		if (node is FunctionCall)
		{
			FunctionCall functionCall = (FunctionCall)node;
			Function funcDecl = functions.Find(v => v.identifier == ((FunctionCall)node).Identifier);
			if (funcDecl == default)
				throw new CompileError($"Syntax Error: {((FunctionCall)node).Identifier} does not exist within this context!");
			functionCall.Size = funcDecl.size;
			if (funcDecl.args.Count != functionCall.Arguments.Count)
				throw new CompileError($"Syntax Error: argument count of function {functionCall.Identifier} does not match!");
			for (int i = 0; i < functionCall.Arguments.Count; i++)
			{
				if (functionCall.Arguments[i].Type != funcDecl.args[i])
					throw new CompileError($"Type Error: {functionCall.Identifier} argument {i} is not of type {funcDecl.args[i]}!");
			}
		}
		if (node is ReturnStatement)
		{
			ReturnStatement returnStatement = (ReturnStatement)node;
			// This next code is horrible, im warning you
			Node parent = GetNodeContext(returnStatement).parent;
			if (parent is FunctionDeclaration)
			{
				if (returnStatement.ReturnValue.Type != ((FunctionDeclaration)parent).Type)
					throw new CompileError($"Type Error: returned type is different to return type of function!");
			}
		}
		if (node is BreakStatement)
		{
			BreakStatement breakStatement = (BreakStatement)node;
			// TODO make sure break statement is within while loop (later release)
		}
	}

	private Context GetNodeContext(Node node)
	{
		Node current = node;
		while (!(current is Context))
		{
			current = current.parent;
		}
		return (Context)current;
	}

	private void SetParent(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			child.parent = node;
			SetParent(child);
		}
	}

	private string ResolveType(Expression expression)
	{
		if (expression is FunctionCall)
		{
			return expression.Type;
		}
		if (expression is ConstantValue)
		{
			return expression.Type;
		}
		if (expression is DeclaredVariable)
		{
			return expression.Type;
		}
		if (expression is SingleOperandExpression)
		{
			return ResolveType(((SingleOperandExpression)expression).Value);
		}
		if (expression is TwoOperandExpression)
		{
			TwoOperandExpression twoOperand = (TwoOperandExpression)expression;
			switch (twoOperand.Operator)
			{
				case "<":
					if (twoOperand.Left.Type != "int" || twoOperand.Right.Type != "int")
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case ">":
					if (twoOperand.Left.Type != "int" || twoOperand.Right.Type != "int")
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case "&&":
					if (twoOperand.Left.Type != "bool" || twoOperand.Right.Type != "bool")
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case "||":
					if (twoOperand.Left.Type != "bool" || twoOperand.Right.Type != "bool")
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case "==":
					if (twoOperand.Left.Type != twoOperand.Right.Type)
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case "!=":
					if (twoOperand.Left.Type != twoOperand.Right.Type)
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case "+":
					if (twoOperand.Left.Type != "int" || twoOperand.Right.Type != "int")
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				case "-":
					if (twoOperand.Left.Type != "int" || twoOperand.Right.Type != "int")
						throw new CompileError($"Type Error: operands of < must both be integers");
					break;
				}
			switch (((TwoOperandExpression)expression).Operator)
			{
				case "<":
					return "bool";
				case ">":
					return "bool";
				case "&&":
					return "bool";
				case "||":
					return "bool";
				case "==":
					return "bool";	
				case "!=":
					return "bool";
				case "+":
					return "int";
				case "-":
					return "int";
				case "&":
					return "int";
				case "|":
					return "int";
				case "^":
					return "int";
				default:
					throw new CompileError($"Syntax Error: {((TwoOperandExpression)expression).Operator} is not a valid operator!");
			}
		}
		throw new CompileError($"Syntax Error: type of {expression.Type} cannot be found");
	}
}