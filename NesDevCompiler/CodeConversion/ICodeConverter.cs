using Mossy.Parser.AbstractSyntaxTree;

namespace Mossy.CodeConversion;

public interface ICodeConverter
{
	public string Convert(Node root);
}