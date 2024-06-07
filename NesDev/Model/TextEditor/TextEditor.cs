using System.Collections;
using System.Linq;

namespace Liken.Model.TextEditor;

public class TextEditor : ICloneable, IEquatable<TextEditor>, IEnumerable<char>, IEnumerable<string>
{
	private string _buffer;

	private int _cursor;
	private int _cursorPos
	{
		get => _cursor;
		set
		{
			_cursor = value;
			if (_cursorPos < 0)
			{
				_cursor = 0;
				UpdateIdealPos();
			}
			if (_cursorPos > _buffer.Length)
			{
				_cursor = _buffer.Length;
				UpdateIdealPos();
			}
		}
	}
	private int _idealPos;
	private void UpdateIdealPos() => _idealPos = CursorPositionInLine;

	private Selection _selection;

	public string Buffer => _buffer;
	public int CursorPos => _cursorPos;
	public string[] Lines => Buffer.Split('\n');

	/// <summary>
	/// Returns the line number the cursor is currently in
	/// </summary>
	/// <remarks>Returns the line number 0 indexed so it can be used in arrays</remarks>
	public int Line
	{
		get
		{
			string[] lines = Lines;
			int len = 0;
			for (int i = 0; i < lines.Length; i++)
			{
				len += lines[i].Length + 1;
				if (len > CursorPos)
				{
					return i;
				}
			}
			return 0;
		}
	}
	public int CursorPositionInLine
	{
		get
		{
			int line = Line;
			int count = 0;
			for (int i = 0; i < line; i++)
			{
				count += GetLineLength(i) + 1;
			}
			return CursorPos - count;
		}
	}

	public int GetLineLength(int line)
	{
		string[] lines = Lines;
		if (line >= lines.Length)
			return 0;
		if (line < 0)
			return 0;

		return lines[line].Length;
	}

	public TextEditor(string text = "", int cursorPos = 0)
	{
		_buffer = text;
		_cursorPos = cursorPos;
	}

	#region Editing
	public void AddAtCursor(char c)
	{
		_buffer = _buffer.Insert(CursorPos, c.ToString());
		MoveRight();
	}

	public void AddAtCursor(string str)
	{
		_buffer = _buffer.Insert(CursorPos, str);
		MoveRight(str.Length);
	}

	public void RemoveAtCursor()
	{
		if (_selection.length > 0)
		{
			_buffer = _buffer.Remove(_selection.startIndex, _selection.length);
			_cursorPos = _selection.startIndex;
			UpdateIdealPos();
			_selection = default;
		}
		else
		{
			if (CursorPos == 0)
				return;
			_buffer = _buffer.Remove(CursorPos - 1, 1);
			MoveLeft();
		}
	}

	public void SetSelection(int startIndex, int endIndex)
	{
		_selection = new Selection(startIndex, endIndex);
	}
	#endregion

	#region Cursor Movement
	public void SetCursorPos(int pos)
	{
		_cursorPos = pos;
		SetSelection(pos, pos);
	}

	public void MoveUp()
	{
		int i = Line;
		int pos = CursorPositionInLine;
		if (GetLineLength(i - 1) < pos)
		{
			_cursorPos -= pos + 1; // beginning of line + 1
		}
		else
		{
			_cursorPos -= GetLineLength(i - 1) + 1;
			int newPos = CursorPositionInLine;
			if (_idealPos >= newPos)
			{
				if (_idealPos > GetLineLength(Line))
				{
					_cursorPos += GetLineLength(Line) - newPos;
				}
				else
				{
					_cursorPos += _idealPos - newPos;
				}
			}
		}
	}

	public void MoveDown()
	{
		int i = Line;
		int pos = CursorPositionInLine;
		if (GetLineLength(i + 1) < pos)
		{
			_cursorPos += GetLineLength(i) - pos + GetLineLength(i + 1) + 1; // end of line + 1
		}
		else
		{
			_cursorPos += GetLineLength(i) + 1;
			int newPos = CursorPositionInLine;
			if (_idealPos >= newPos)
			{
				if (_idealPos > GetLineLength(Line))
				{
					_cursorPos += GetLineLength(Line) - newPos;
				}
				else
				{
					_cursorPos += _idealPos - newPos;
				}
			}
		}
	}

	public void MoveLeft(int amount = 1)
	{
		_cursorPos -= amount;
		UpdateIdealPos();
	}

	public void MoveRight(int amount = 1)
	{
		_cursorPos += amount;
		UpdateIdealPos();
	}
	#endregion

	#region Interfaces
	public object Clone()
	{
		return new TextEditor(_buffer, _cursorPos);
	}

	public bool Equals(TextEditor? other)
	{
		if (other == null)
			return false;

		return Buffer == other.Buffer;
	}

	public IEnumerator<char> GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return Buffer.GetEnumerator();
	}

	IEnumerator<string> IEnumerable<string>.GetEnumerator()
	{
		return Lines.ToList().GetEnumerator();
	}
	#endregion
}