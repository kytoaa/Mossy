using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.CodeConversion;

public class Assembly6502CodeConverter: ICodeConverter
{
	private readonly string[] HexadecimalNumbers = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

	public string Convert(Node root)
	{
		string asm = SetupBase();
		asm += ConvertContext((Context)root);
		return asm;
	}

	private string SetupBase()
	{
		string utils = "";

		utils += @"sys_create_context:
  clc
  ldx $00
  txa
  adc $00, x ; 5, 0, 0, 0, 0, 3, 1, 2, z
  sta $00
  tay               ; stores new value of $00
  txa               ; now a is initial value of $00
  sta $01, y        ; stores previous value of context pointer to new context
  tya               ; now a is new value of $00
  tax
  lda $02
  sta $00, x
  rts

sys_clear_context:
  ldx $00
  ldy $01, x
  sty $00
  rts";

		return utils;
	}

	private string ConvertContext(Context context)
	{
		string contextString = "";

		foreach (VariableDeclaration variable in context.variables)
		{
			contextString += ConvertVariable(variable);
		}
		foreach (FunctionDeclaration functionDeclaration in context.functions)
		{
			contextString += "\n";
			contextString += $"nesdev_{functionDeclaration.Identifier}:" + "\n";
			contextString += ConvertContext(functionDeclaration.Body);
			contextString += "rts" + "\n";
		}

		return contextString;
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
		string statementString = "";

		if (statement is FunctionCall)
		{
			statementString += ConvertExpression((FunctionCall)statement);
			statementString += "pla" + "\n";
			return statementString;
		}
		else if (statement is VariableAssignent)
		{
			VariableAssignent variableAssignent = (VariableAssignent)statement;

			statementString += ConvertExpression(variableAssignent.Expression);

			statementString += "pla" + "\n";
			statementString += "ldx $00" + "\n";
			statementString += $"sta {ConvertToHex(variableAssignent.Address + 3)}, x" + "\n";
			return statementString;
		}
		else if (statement is IfStatement)
		{
			IfStatement ifStatement = (IfStatement)statement;
		}
		else if (statement is WhileStatement)
		{
			WhileStatement whileStatement = (WhileStatement)statement;
		}
		else if (statement is ReturnStatement)
		{
			ReturnStatement returnStatement = (ReturnStatement)statement;
		}


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
			string assignment = ConvertExpression(var.Assignent.Expression);
			result += assignment;
			result += "pla" + "\n";
		}
		else
		{
			result += "lda #$00" + "\n";
		}
		if (var.IsGlobal)
		{
			result += $"sta ${ConvertToHex(var.Address + 4)}" + "\n";
		}
		else
		{
			result += $"sta ${ConvertToHex(var.Address + 3)}" + "\n";
		}

		return result;
	}

	private string ConvertExpression(Expression expression)
	{
		if (expression is DeclaredVariable)
		{
			DeclaredVariable declaredVariable = (DeclaredVariable)expression;
			string value = "";
			if (declaredVariable.IsGlobal)
			{
				value += $"lda ${ConvertToHex(declaredVariable.Address + 4)}";
				value += "\n";
				value += "pha";
				value += "\n";
			}
			else
			{
				value += "ldx $00";
				value += "\n";
				value += $"lda ${ConvertToHex(declaredVariable.Address + 3)}, x";
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

			string value = "";

			// TODO function call code
			// set arguments
			for (int i = 0; i < functionCall.Arguments.Count; i++)
			{
				value += ConvertExpression(functionCall.Arguments[i]);
				value += "lda $00" + "\n";
				value += "adc $00, x" + "\n";
				value += "pla" + "\n";
				value += $"sta ${ConvertToHex(i + 3)}, x" + "\n";
			}
			// jump to context
			value += $"lda #${ConvertToHex(functionCall.Size + 3)}" + "\n";
			value += "sta $02" + "\n";
			value += "jsr sys_create_context" + "\n";
			value += $"jsr nesdev_{functionCall.Identifier}" + "\n";
			// close context
			value += $"jsr sys_close_context" + "\n";
			value += "pha" + "\n";
		}

		throw new NotImplementedException();
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