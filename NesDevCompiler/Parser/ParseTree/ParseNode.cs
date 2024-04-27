namespace NesDevCompiler.Parser.ParseTree;

public class ParseNode
{
	public string Value = "";

	public ParseNode Parent;

	public List<ParseNode> Children = new List<ParseNode>();

	public ParseNode(ParseNode parent)
	{
		Parent = parent;
	}
}