namespace NesDevCompiler.CodeConversion;

public static class AssemblyOperations
{
	public static string Add()
	{
		string add = @"clc
adc $03";
		return add + "\n";
	}

	public static string Subtract()
	{
		// TODO replace with correct flag
		string subtract = @"sec
sbc $03";
		return subtract + "\n";
	}

	public static string GreaterThan()
	{
		string greaterThan = @"cmp $03
bcs :+
lda #$00
jmp :++
:
lda #$01
:";
		return greaterThan + "\n";
	}

	public static string LessThan()
	{
		string lessThan = @"cmp $03
bcc :+
lda #$00
jmp :++
:
lda #$01
:";
		return lessThan + "\n";
	}

	public static string EqualTo()
	{
		string equalTo = @"cmp $03
beq :+
lda #$00
jmp :++
:
lda #$01
:";
		return equalTo + "\n";
	}

	public static string NotEqual()
	{
		string notEqual = @"cmp $03
bne :+
lda #$00
jmp :++
:
lda #$01
:";
		return notEqual + "\n";
	}

	public static string Not()
	{
		string not = @"ldx #$ff
stx $03
eor $03";
		return not + "\n";
	}

	public static string And()
	{
		string and = @"and $03";
		return and + "\n";
	}

	public static string Or()
	{
		string or = @"ora $03";
		return or + "\n";
	}

	public static string BitwiseAnd()
	{
		string and = @"and $03";
		return and + "\n";
	}

	public static string BitwiseOr()
	{
		string or = @"ora $03";
		return or + "\n";
	}

	public static string BitwiseXOr()
	{
		string xOr = @"eor $03";
		return xOr + "\n";
	}

	public static string BitshiftLeft()
	{
		string shiftLeft = @"asl";
		return shiftLeft + "\n";
	}

	public static string BitshiftRight()
	{
		string shiftRight = @"lsr";
		return shiftRight + "\n";
	}
}