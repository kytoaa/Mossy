using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.CodeConversion;

public class Assembly6502CodeConverter: ICodeConverter
{
	private readonly string[] HexadecimalNumbers = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

	private int labelCount = 0;// { get { labelCount += 1; return labelCount; } set { labelCount = value; } }

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

		return utils + "\n";
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
		foreach (IStatement statement in context.statements)
		{
			contextString += "\n";
			contextString += ConvertStatement(statement);
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

	private string ConvertStatement(IStatement statement)
	{
		string statementString = "";

		if (statement is FunctionCall)
		{
			statementString += ConvertExpression((FunctionCall)statement);
			statementString += "pla" + "\n";
			return statementString + "\n";
		}
		else if (statement is VariableAssignent)
		{
			VariableAssignent variableAssignent = (VariableAssignent)statement;

			statementString += ConvertExpression(variableAssignent.Expression);

			statementString += "pla" + "\n";
			if (variableAssignent.IsGlobal)
			{
				statementString += $"; global var {variableAssignent.Identifier}" + "\n";
				statementString += $"sta ${ConvertToHex(variableAssignent.Address + 4)}" + "\n";
			}
			else
			{
				statementString += $"; local var {variableAssignent.Identifier}" + "\n";
				statementString += "ldx $00" + "\n";
				statementString += $"sta {ConvertToHex(variableAssignent.Address + 3)}, x" + "\n";
			}

			return statementString + "\n";
		}
		else if (statement is IfStatement)
		{
			statementString += "; converting if statement" + "\n";
			IfStatement ifStatement = (IfStatement)statement;
			statementString += ConvertExpression(ifStatement.Condition);
			int label = labelCount;
			labelCount += 1;
			statementString += @$"pla
ldx #$01
stx $03
cmp $03
bne if_statement_{label}" + "\n";
			foreach (IStatement bodyStatement in ifStatement.Body)
			{
				statementString += ConvertStatement(bodyStatement);
			}
			if (ifStatement.ElseBody != null)
				statementString += $"jmp if_statement_else_{label}" + "\n";
			statementString += $"if_statement_{label}:" + "\n";
			if (ifStatement.ElseBody != null)
			{
				foreach (IStatement elseBodyStatement in ifStatement.ElseBody)
				{
					statementString += ConvertStatement(elseBodyStatement);
				}
			}
			statementString += $"if_statement_else_{label}:" + "\n";
			return statementString + "\n";
		}
		else if (statement is WhileStatement)
		{
			statementString += "; converting while statement" + "\n";
			WhileStatement whileStatement = (WhileStatement)statement;
			int label = labelCount;
			labelCount += 1;
			statementString += $"while_statement_{label}:" + "\n";
			statementString += ConvertExpression(whileStatement.Condition);
			statementString += @$"pla
ldx #$01
stx $03
cmp $03
bne while_statement_end_{label}" + "\n";
			foreach (IStatement whileStatementBody in  whileStatement.Body)
			{
				statementString += ConvertStatement(whileStatementBody);
			}
			statementString += $"jmp while_statement_{label}" + "\n";
			statementString += $"while_statement_end_{label}:" + "\n";
			return statementString;
		}
		else if (statement is ReturnStatement)
		{
			ReturnStatement returnStatement = (ReturnStatement)statement;
			// TODO return
			return "; return statement" + "\n";
		}
		else if (statement is BreakStatement)
		{
			BreakStatement breakStatement = (BreakStatement)statement;
			// TODO break
		}


		throw new NotImplementedException();
	}

	private string ConvertVariable(VariableDeclaration var)
	{
		string result = "";
		result += $"; converting variable {var.Identifier}" + "\n";

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
			result += "ldx $00" + "\n";
			result += $"sta ${ConvertToHex(var.Address + 3)}, x" + "\n";
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
			value += GetASMOperation(twoOperandExpression.Operator) + "\n";
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
			value += $"jsr sys_clear_context" + "\n";
			value += "pha" + "\n";
			// TODO might be more to do here, also might not work
			return value;
		}

		throw new NotImplementedException();
	}

	private string GetASMOperation(string operation)
	{
		switch (operation)
		{
			case "+":
				return AssemblyOperations.Add();
			case "-":
				return AssemblyOperations.Subtract();
			case ">":
				return AssemblyOperations.GreaterThan();
			case "<":
				return AssemblyOperations.LessThan();
			case "==":
				return AssemblyOperations.EqualTo();
			case "!=":
				return AssemblyOperations.NotEqual();
			case "&&":
				return AssemblyOperations.And();
			case "||":
				return AssemblyOperations.Or();
			case "&":
				return AssemblyOperations.BitwiseAnd();
			case "|":
				return AssemblyOperations.BitwiseOr();
			case "^":
				return AssemblyOperations.BitwiseXOr();
			case "<<":
				return AssemblyOperations.BitshiftLeft();
			case ">>":
				return AssemblyOperations.BitshiftRight();
			default:
				throw new NotImplementedException();
		}

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