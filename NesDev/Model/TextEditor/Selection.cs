namespace Liken.Model.TextEditor;

/// <summary>
/// Represents a selection within the text editor
/// </summary>
public struct Selection
{
	public int startIndex;
	public int endIndex;
	public int length => endIndex - startIndex;

	public Selection(int startIndex, int endIndex)
	{
		this.startIndex = startIndex;
		this.endIndex = endIndex;
	}

	string GetSelection(string str) => str.Substring(startIndex, length);
}