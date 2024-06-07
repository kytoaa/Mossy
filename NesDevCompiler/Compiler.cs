using System;
using System.Diagnostics;

namespace Mossy;

public class Compiler
{
	private CodeConversion.ICodeConverter _codeConverter;

	public Compiler(params string[] settings)
	{
		switch (settings[0])
		{
			case "6502":
				_codeConverter = new CodeConversion.Assembly6502CodeConverter();
				break;
			case "python":
				_codeConverter = new CodeConversion.PythonCodeConverter();
				break;
			default:
				throw new ArgumentException("Not a valid compiler type");
		}
	}

	public bool TryCompile(string code, out string compiledText)
	{
		List<Processors.ILexerProcessor> lexerProcessors = new List<Processors.ILexerProcessor>() { new Processors.ConstantProcessor() };

		Lexer.ILexer lexer = new Lexer.Lexer(new CharacterStream.CharacterStream(code), lexerProcessors);

		Parser.AbstractSyntaxTree.Node? node = null;
		try
		{
			node = new Parser.Parser().Parse(lexer);
			Debug.WriteLine(node);
		}
		catch (Exception ex)
		{
			compiledText = ex.Message;
			return false;
		}

		if (node != null)
		{
			try
			{
				compiledText = _codeConverter.Convert(node);
			}
			catch (Exception ex)
			{
				compiledText = ex.Message;
				return false;
			}
			return true;
		}
		compiledText = "Compilation Failed!";
		return false;
	}
}