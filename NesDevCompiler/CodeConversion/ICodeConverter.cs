using NesDevCompiler.Parser.AbstractSyntaxTree;

namespace NesDevCompiler.CodeConversion;

public interface ICodeConverter
{
	public string Convert(Node root);
}