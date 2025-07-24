using Mossy.Parser.AbstractSyntaxTree;

namespace Mossy.CodeConversion;

public class Assembly6502CodeConverter: ICodeConverter
{
	private readonly string[] HexadecimalNumbers = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

	private int labelCount = 0;// { get { labelCount += 1; return labelCount; } set { labelCount = value; } }

  private bool useDefaultResetFunc = true;
  private bool useDefaultNMIFunc = true;
  private bool useDefaultIRQFunc = true;
  private bool useMainFunc = false;

  private string defaultResetFunc = @"
mossy_reset:
  sei		; disable IRQs
  cld		; disable decimal mode
  ldx #$40
  stx $4017	; disable APU frame IRQ
  ldx #$ff 	; Set up stack
  txs		;  .
  inx		; now X = 0
  stx $2000	; disable NMI
  stx $2001 	; disable rendering
  stx $4010 	; disable DMC IRQs

;; first wait for vblank to make sure PPU is ready
vblankwait1:
  bit $2002
  bpl vblankwait1

clear_memory:
  lda #$00
  sta $0000, x
  sta $0100, x
  sta $0200, x
  sta $0300, x
  sta $0400, x
  sta $0500, x
  sta $0600, x
  sta $0700, x
  inx
  bne clear_memory

;; second wait for vblank, PPU is ready after this
vblankwait2:
  bit $2002
  bpl vblankwait2

lda #$80
sta $2000 ; enable NMI

<resetAppendString>

jmp mossy_forever

";
  private string defaultIRQFunc = @"
mossy_irq:
  rti
";

  private string[] resetIdentifiers = {"reset", "Reset"};
  private string[] nmiIdentifiers = {"nmi", "NMI", "NonMaskableInterrupt"};
  private string[] mainIdentifiers = {"main", "Main"};
  private string[] irqIdentifiers = {"irq", "IRQ", "InterruptRequest"};

	public string Convert(Node root)
	{
		string asm = "";
		asm += SetupBase();
		asm += ConvertContext((Context)root, true) + "\n";
		asm += SetupSys();
		return asm;
	}

	private string SetupBase()
	{
		string value = "";
		value += @".segment ""HEADER""
  ; .byte ""NES"", $1A      ; iNES header identifier
  .byte $4E, $45, $53, $1A
  .byte 2               ; 2x 16KB PRG code
  .byte 1               ; 1x  8KB CHR data
  .byte $01, $00        ; mapper 0, vertical mirroring

.segment ""VECTORS""
  ;; When an NMI happens (once per frame if enabled) the label nmi:
  .addr mossy_nmi
  ;; When the processor first turns on or is reset, it will jump to the label reset:
  .addr mossy_reset
  ;; External interrupt IRQ
  .addr mossy_irq

; ""nes"" linker config requires a STARTUP section, even if it's empty
.segment ""STARTUP""

; Main code segment for the program
.segment ""CODE""

";

		return value;
	}

	private string SetupSys()
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
  rts

mossy_forever:
  jmp mossy_forever

.segment ""CHARS""";

		return utils + "\n";
	}

	private string ConvertContext(Context context, bool isGlobal = false)
	{
		string contextString = "" + "\n";
    string resetAppendString = "\n";
		if (isGlobal)
		{
      // useDefaultResetFunc =
      //   (context.functions.Any(func => func.Identifier == "Reset" || func.Identifier == "reset"));
      // useDefaultNMIFunc =
      //   (context.functions.Any(func => func.Identifier == "NMI" || func.Identifier == "nmi"));
      // useDefaultIRQFunc =
      //   (context.functions.Any(func => func.Identifier == "IRQ" || func.Identifier == "irq"));
			resetAppendString += "\n";
			resetAppendString += "; setting up global context" + "\n";
			resetAppendString += $@"lda #${ConvertToHex(context.variables.Sum(v => v.Size) + 4)}
sta $00
lda #${ConvertToHex(context.variables.Sum(v => v.Size) + 3)}
sta $01";
			resetAppendString += "\n";
		}

		foreach (VariableDeclaration variable in context.variables)
		{
			if (isGlobal)
			{
				resetAppendString += ConvertVariable(variable);
			}
		}
		if (isGlobal)
		{
			List<FunctionDeclaration> nmiFuncs = context.functions.Where(f => nmiIdentifiers.Contains(f.Identifier)).ToList();
			List<FunctionDeclaration> mainFuncs = context.functions.Where(f => mainIdentifiers.Contains(f.Identifier)).ToList();
			if (nmiFuncs.Count > 1)
				throw new Exception("Compile Error: Too many NMI functions! Try including only one");
			if (mainFuncs.Count > 1)
				throw new Exception("Compile Error: Too many Main functions! Try including only one");
      if (nmiFuncs.Count + mainFuncs.Count == 0)
        throw new Exception("Compile Error: Could find neither an NMI nor Main function!");

      useDefaultNMIFunc = nmiFuncs.Count == 1;
      useMainFunc = mainFuncs.Count == 1;

      switch ((mainFuncs.Count() << 1) + nmiFuncs.Count()) { // 1 = NMI only, 2 = Main only, 3 = Both
        case 1:
          FunctionDeclaration nmiFunc = nmiFuncs[0];
          // contextString += $"lda #${ConvertToHex(nmiFunc.Size + 3)}" + "\n";
          // contextString += "sta $02" + "\n";
          // contextString += "jsr sys_create_context" + "\n";
          // contextString += $"jsr mossy_nmi" + "\n";
          // // close context
          // contextString += $"jsr sys_clear_context" + "\n" + "\n";
          // contextString += "jmp mossy_forever" + "\n" + "\n";

          contextString += $"mossy_nmi:" + "\n";
          contextString += $"lda #${ConvertToHex(nmiFunc.Size + 3)}" + "\n";
          contextString += "sta $02" + "\n";
          contextString += "jsr sys_create_context" + "\n";
          contextString += ConvertContext(nmiFunc.Body);
          contextString += $"jsr sys_clear_context" + "\n" + "\n";
          contextString += "rti" + "\n";
          break;
        case 2:
          throw new Exception("Compile Error: No NMI function detected!"); // may include support for default nmi function later
          //break;
        case 3:
          throw new NotImplementedException("TODO: Work on including Main and NMI together");
        default:
          break;
      }
			// FunctionDeclaration mainFunc = mainFuncs[0];
			// contextString += $"lda #${ConvertToHex(mainFunc.Size + 3)}" + "\n";
			// contextString += "sta $02" + "\n";
			// contextString += "jsr sys_create_context" + "\n";
			// contextString += $"jsr mossy_{mainFunc.Identifier}" + "\n";
			// // close context
			// contextString += $"jsr sys_clear_context" + "\n" + "\n";
			// contextString += "jmp mossy_forever" + "\n";
		}
		foreach (FunctionDeclaration functionDeclaration in context.functions)
		{
      if (nmiIdentifiers.Contains(functionDeclaration.Identifier) || mainIdentifiers.Contains(functionDeclaration.Identifier))
        continue;
			contextString += "\n";

      if (resetIdentifiers.Contains(functionDeclaration.Identifier)) {
        contextString += $"mossy_reset:" + "\n";
        useDefaultResetFunc = false;
      }
      else if (irqIdentifiers.Contains(functionDeclaration.Identifier)) {
        contextString += $"mossy_irq:" + "\n";
        useDefaultIRQFunc = false;
      }
      else contextString += $"mossy_{functionDeclaration.Identifier}:" + "\n";

			contextString += ConvertContext(functionDeclaration.Body);

      if (resetIdentifiers.Contains(functionDeclaration.Identifier))
        contextString = contextString.Replace("<resetAppendString>", resetAppendString);
      else if (irqIdentifiers.Contains(functionDeclaration.Identifier)) {}
      else {
        contextString += "lda #$00" + "\n";
        contextString += "rts" + "\n";
      }
		}
    if (isGlobal) {
      if (useDefaultResetFunc)
        contextString += defaultResetFunc.Replace("<resetAppendString>", resetAppendString);
      if (useDefaultIRQFunc)
        contextString += defaultIRQFunc;
    }

		foreach (IStatement statement in context.statements)
		{
			contextString += "\n";
			contextString += ConvertStatement(statement);
		}

		return contextString;
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

			statementString += ConvertExpression(variableAssignent.Expression) + "\n";

			statementString += "ldx #$00" + "\n";
			if (variableAssignent.Offset != null)
			{
				statementString += $"; calculating offset for {variableAssignent.Identifier}" + "\n";
				statementString += ConvertExpression(variableAssignent.Offset) + "\n";
				statementString += $"; applying offset for {variableAssignent.Identifier}" + "\n";
				if (variableAssignent.IsGlobal)
				{
					statementString += @"pla
tax" + "\n";
				}
				else
				{
					statementString += @"pla
clc
adc $00
tax" + "\n";
				}
			}

			statementString += "pla" + "\n";
			if (variableAssignent.IsGlobal)
			{
				statementString += $"; global var {variableAssignent.Identifier}" + "\n";
				statementString += $"sta ${ConvertToHex(variableAssignent.Address + 4)}, x" + "\n";
			}
			else
			{
				statementString += $"; local var {variableAssignent.Identifier}" + "\n";
				if (variableAssignent.Offset == null)
				{
					statementString += "; no offset but local, offsetting by context pointer" + "\n";
					statementString += "ldx $00" + "\n";
				}
				else
				{
					statementString += "; has local offset" + "\n";
				}
				statementString += $"sta ${ConvertToHex(variableAssignent.Address + 3)}, x" + "\n";
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

			statementString += "; return statement" + "\n";
			statementString += ConvertExpression(returnStatement.ReturnValue) + "\n";
			statementString += "pla" + "\n";
			statementString += "rts" + "\n";
			return statementString;
		}
		else if (statement is BreakStatement)
		{
			BreakStatement breakStatement = (BreakStatement)statement;
			// TODO break (later release)
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
			value += "ldx #$00" + "\n";
			if (declaredVariable.Offset != null)
			{
				value += $"; calculating offset for {declaredVariable.Identifier}" + "\n";
				value += ConvertExpression(declaredVariable.Offset) + "\n";
				value += $"; applying offset for {declaredVariable.Identifier}" + "\n";
				if (declaredVariable.IsGlobal)
				{
					value += @"pla
tax" + "\n";
				}
				else
				{
					value += @"pla
clc
adc $00
tax" + "\n";
				}

			}
			if (declaredVariable.IsGlobal)
			{
				value += $"lda ${ConvertToHex(declaredVariable.Address + 4)}, x";
				value += "\n";
				value += "pha";
				value += "\n";
			}
			else
			{
				if (declaredVariable.Offset == null)
				{
					value += "; no offset but local, offsetting by context pointer" + "\n";
					value += "ldx $00" + "\n";
				}
				else
				{
					value += "; has local offset" + "\n";
				}
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

			// set arguments
			for (int i = 0; i < functionCall.Arguments.Count; i++)
			{
				value += ConvertExpression(functionCall.Arguments[i]);
				value += "lda $00" + "\n";
				value += "tax" + "\n";
				value += "adc $00, x" + "\n";
				value += "tax" + "\n";
				value += "pla" + "\n";
				value += $"sta ${ConvertToHex(i + 3)}, x" + "\n";
			}
			// jump to context
			value += $"lda #${ConvertToHex(functionCall.Size + 3)}" + "\n";
			value += "sta $02" + "\n";
			value += "jsr sys_create_context" + "\n";
			value += $"jsr mossy_{functionCall.Identifier}" + "\n";
			// close context
			value += $"jsr sys_clear_context" + "\n";
			value += "pha" + "\n";
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
