using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using System.IO;

namespace Liken.ViewModel;

public class TextEditorViewModel : INotifyPropertyChanged
{
	private string _text = "";
	private string _output = "";
	public string Text 
	{
		get => _text;
		set
		{
			_text = value;
			SetProperty();
		}
	}
	public string Output
	{
		get => _output;
		set
		{
			_output = value;
			SetProperty();
		}
	}

	private Model.TextEditor.TextEditor editor;

	public TextEditorViewModel()
	{
		editor = new Model.TextEditor.TextEditor();
		Output = "Hello, World!";
	}
	public TextEditorViewModel(string filePath)
	{
		editor = new Model.TextEditor.TextEditor(OpenFile(filePath));
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetProperty([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void Initialize()
	{
		Text = editor.Buffer;
	}

	public string OpenFile(string path)
	{
		if (Path.Exists(path))
		{
			return File.ReadAllText(path);
		}
		Output = "File does not exist!";
		return "";
	}

	public void OnTextChanged(TextEvent e, Action handler, Action<int> setCaret)
	{
		Debug.WriteLine(e.Text);
		switch (e.Type)
		{
			case TextEvent.EventType.Space:
				editor.AddAtCursor(" ");
				break;
			case TextEvent.EventType.Enter:
				editor.AddAtCursor("\n");
				break;
			case TextEvent.EventType.Tab:
				editor.AddAtCursor("\t");
				break;
			case TextEvent.EventType.Backspace:
				editor.RemoveAtCursor();
				break;
			case TextEvent.EventType.Left:
				editor.MoveLeft();
				break;
			case TextEvent.EventType.Right:
				editor.MoveRight();
				break;
			case TextEvent.EventType.Up:
				editor.MoveUp();
				break;
			case TextEvent.EventType.Down:
				editor.MoveDown();
				break;
			case TextEvent.EventType.CursorPos:
				if (int.TryParse(e.Text, out int i))
				{
					editor.SetCursorPos(i);
				}
				else
				{
					throw new Exception("Index passed is not a number");
				}
				break;
			case TextEvent.EventType.Text:
				if (e.Text == null)
					throw new Exception("Text is null");
				HandleText(e.Text);
				break;
		}
		handler.Invoke();
		Text = editor.Buffer;
		setCaret.Invoke(editor.CursorPos);
	}
	private void HandleText(string text)
	{
		switch (text)
		{
			case "	":
				editor.AddAtCursor('	');
				break;
			case "{":
				editor.AddAtCursor("{");
				editor.AddAtCursor("}");
				editor.MoveLeft();
				break;
			case "(":
				editor.AddAtCursor("(");
				editor.AddAtCursor(")");
				editor.MoveLeft();
				break;
			default:
				editor.AddAtCursor(text);
				break;
		}

	}

}