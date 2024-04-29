using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.CodeConversion;

public class PythonCodeConverter : ICodeConverter
{
	public string Convert(Node root)
	{
		return ConvertContext((Context)root, 0);
	}

	private string GetIndent(int depth)
	{
		string indent = "";
		for (int i = 0; i < depth; i++)
		{
			indent += "    ";
		}
		return indent;
	}

	private string ConvertContext(Context context, int depth)
	{
		string contextString = "";
		foreach (VariableDeclaration variableDeclaration in context.variables)
		{
			if (depth != 0)
				break;
			contextString += GetIndent(depth);
			contextString += $"{variableDeclaration.Identifier}: {variableDeclaration.Type}";
			if (depth == 0 && (variableDeclaration.Assignent != null))
			{
				contextString += $" = {ConvertExpression(variableDeclaration.Assignent.Expression)}";
			}
			contextString += "\n";
		}
		foreach (FunctionDeclaration functionDeclaration in context.functions)
		{
			//contextString += GetIndent(depth);
			contextString += "def ";
			contextString += functionDeclaration.Identifier;
			contextString += "(";
			for (int i = 0; i < functionDeclaration.Arguments.Count; i++)
			{
				if (i > 0)
					contextString += ", ";
				contextString += ConvertVariable(functionDeclaration.Arguments[i]);
			}
			contextString += "):";
			contextString += "\n";
			contextString += ConvertContext(functionDeclaration.Body, depth + 1);
		}
		foreach (IStatement statement in context.statements)
		{
			contextString += ConvertStatement(statement, depth);
		}
		return contextString;
	}

	private string ConvertStatement(IStatement statement, int depth)
	{
		string statementString = GetIndent(depth);
		if (statement is FunctionCall)
		{
			statementString += ConvertExpression((FunctionCall)statement);
		}
		else if (statement is VariableAssignent)
		{
			VariableAssignent variableAssignent = (VariableAssignent)statement;
			statementString += $"{variableAssignent.Identifier} = {ConvertExpression(variableAssignent.Expression)}";
		}
		else if (statement is IfStatement)
		{
			IfStatement ifStatement = (IfStatement)statement;
			statementString += $"if {ConvertExpression(ifStatement.Condition)}:";
			statementString += "\n";
			foreach (IStatement bodyStatement in ifStatement.Body)
			{
				statementString += ConvertStatement(bodyStatement, depth + 1);
			}
			if (ifStatement.ElseBody != null)
			{
				statementString += GetIndent(depth);
				statementString += "else:";
				statementString += "\n";
				foreach (IStatement elseBodyStatement in ifStatement.ElseBody)
				{
					statementString += ConvertStatement(elseBodyStatement, depth + 1);
				}
			}
		}
		else if (statement is WhileStatement)
		{
			WhileStatement whileStatement = (WhileStatement)statement;
			statementString += $"while {ConvertExpression(whileStatement.Condition)}:";
			statementString += "\n";
			foreach (IStatement bodyStatement in whileStatement.Body)
			{
				statementString += ConvertStatement(bodyStatement, depth + 1);
			}
		}
		else if (statement is ReturnStatement)
		{
			ReturnStatement returnStatement = (ReturnStatement)statement;
			statementString += $"return {ConvertExpression(returnStatement.ReturnValue)}";
		}
		statementString += "\n";
		return statementString;
	}

	private string ConvertVariable(VariableDeclaration var)
	{
		string declaration = var.Identifier;
		declaration += $": {var.Type}";

		if (var.Assignent != null)
		{
			declaration += $" = {ConvertExpression(var.Assignent.Expression)}";
		}

		return declaration;
	}

	private string ConvertExpression(Expression expression)
	{
		string expressionString = "";

		if (expression is DeclaredVariable)
		{
			expressionString += ((DeclaredVariable)expression).Identifier;
		}
		if (expression is ConstantValue)
		{
			string value = ((ConstantValue)expression).Value;
			if (value == "true")
				value = "True";
			if (value == "false")
				value = "False";
			expressionString += value;
		}
		if (expression is SingleOperandExpression)
		{
			string op = ((SingleOperandExpression)expression).Operator;
			if (op == "!")
				op = "not ";
			expressionString += "(";
			expressionString += op;
			expressionString += ConvertExpression(((SingleOperandExpression)expression).Value);
			expressionString += ")";
		}
		if (expression is TwoOperandExpression)
		{
			TwoOperandExpression expr = (TwoOperandExpression)expression;
			expressionString += "(";
			expressionString += ConvertExpression(expr.Left);
			expressionString += expr.Operator;
			expressionString += ConvertExpression(expr.Right);
			expressionString += ")";
		}
		if (expression is FunctionCall)
		{
			FunctionCall functionCall = (FunctionCall)expression;
			expressionString += functionCall.Identifier;
			expressionString += "(";
			for (int i = 0; i < functionCall.Arguments.Count; i++)
			{
				if (i > 0)
					expressionString += ",";
				expressionString += ConvertExpression(functionCall.Arguments[i]);
			}
			expressionString += ")";
		}

		return expressionString;
	}
}