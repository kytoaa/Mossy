namespace NesDevCompiler.Parser.ParseTree;

public class Tree
{
	public ParseNode Current { get; private set; } = new ParseNode(null);

	public Tree AddChild()
	{
		ParseNode node = new ParseNode(Current);
		Current.Children.Add(node);
		Current = node;
		return this;
	}

	public Tree SetCurrentToParent()
	{
		Current = Current.Parent;
		return this;
	}

	public Tree SetValue(string value)
	{
		Current.Value = value;
		return this;
	}
}