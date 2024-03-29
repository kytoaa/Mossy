namespace NesDevCompiler.Parser.ParseTree;

public class CodeBlock : INode
{
	public List<INode> Within = new List<INode>();
}