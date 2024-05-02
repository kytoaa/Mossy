using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.CodeConversion;

public class Assembly6502CodeConverter: ICodeConverter
{
	private readonly string[] HexadecimalNumbers = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

	public string Convert(Node root)
	{
		return ConvertContext((Context)root, 0);
	}

	private string ConvertContext(Context context, int depth)
	{
		throw new NotImplementedException();
		/*
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
		*/
	}

	private string ConvertStatement(IStatement statement, int depth)
	{
		throw new NotImplementedException();
		/*
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
		*/
	}

	private string ConvertVariable(VariableDeclaration var)
	{
		string result = "";

		if (var.Assignent != null)
		{
			//string assignment = var.Assignent.
		}
		else
		{
			
		}

		throw new NotImplementedException();
		/*
		string declaration = var.Identifier;
		declaration += $": {var.Type}";

		if (var.Assignent != null)
		{
			declaration += $" = {ConvertExpression(var.Assignent.Expression)}";
		}

		return declaration;
		*/
	}

	private string ConvertExpression(Expression expression)
	{
		if (expression is DeclaredVariable)
		{
			DeclaredVariable declaredVariable = (DeclaredVariable)expression;
			string value = "";
			if (declaredVariable.IsGlobal)
			{
				value += $"lda ${ConvertToHex(declaredVariable.Address)}";
				value += "\n";
				value += "pha";
				value += "\n";
			}
			else
			{
				value += "ldx $00";
				value += "\n";
				value += $"lda ${ConvertToHex(declaredVariable.Address)}, x";
				value += "\n";
				value += "pha";
				value += "\n";
			}
			return value;
		}
		if (expression is ConstantValue)
		{
			ConstantValue constantValue = (ConstantValue)expression;
			int varVal = 0;
			if (constantValue.Type == "bool")
				varVal += (constantValue.Value == "true") ? 1 : 0;
			else
				varVal = int.Parse(constantValue.Value);
			string value = "";
			value += $"lda #${ConvertToHex(varVal)}";
			value += "\n";
			value += "pha";
			value += "\n";

			return value;
		}
		if (expression is TwoOperandExpression)
		{
			TwoOperandExpression twoOperandExpression = (TwoOperandExpression)expression;

			string value = "";

			string operandl = ConvertExpression(twoOperandExpression.Left);
			string operandr = ConvertExpression(twoOperandExpression.Right);

			value += operandl;
			value += operandr;
			value += "\n";

			value += "pla" + "\n";
			value += "sta $03" + "\n";
			value += "pla" + "\n";
			value += GetASMOperation(twoOperandExpression.Operator) + " $03" + "\n";
			value += "pha" + "\n";
			return value;
		}
		if (expression is SingleOperandExpression)
		{
			SingleOperandExpression singleOperandExpression = (SingleOperandExpression)expression;

			string value = "";

			string operand = ConvertExpression(singleOperandExpression.Value);

			value += operand + "\n";
			value += "pla" + "\n";
			value += GetASMOperation(singleOperandExpression.Operator) + "\n";
			value += "pha" + "\n";
			return value;
		}
		if (expression is FunctionCall)
		{
			FunctionCall functionCall = (FunctionCall)expression;

			// TODO function call code
			// set arguments
			// create context
			// jump to context
			// close context
		}


		throw new NotImplementedException();
		/*
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
		*/
	}

	private string GetASMOperation(string operation)
	{
		throw new NotImplementedException();
	}

	public string ConvertToHex(int num)
	{
		string final = "";
		for (int i = ((num > 255) ? 3 : 1); i >= 0; i--)
		{
			for (int n = 15; n >= 0; n--)
			{
				if ((Math.Pow(16, i) * n) <= num)
				{
					num -= (int)(Math.Pow(16, i) * n);
					final += HexadecimalNumbers[n];
					break;
				}
			}
		}
		return final;
	}
}