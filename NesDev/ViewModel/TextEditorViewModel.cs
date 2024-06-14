using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using System.IO;
using System.Web;
using Liken.Model;

namespace Liken.ViewModel;

public class TextEditorViewModel : IViewModel
{
	//private string _text = "";
	private string _output = "";
	public string Text 
	{
		get => editor.Buffer;
		set
		{
			//_text = value;
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

	private string _path = "";
	private char _prevChar;

	#region Commands
	private ICommand _saveCommand;
	public ICommand SaveCommand
	{
		get
		{
			if (_saveCommand == null)
				_saveCommand = new Command(SaveFile);
			return _saveCommand;
		}
		set
		{
			_saveCommand = value;
			SetProperty();
		}
	}
	private ICommand _saveAsCommand;
	public ICommand SaveAsCommand
	{
		get
		{
			if (_saveAsCommand == null)
				_saveAsCommand = new Command(SaveFileAs);
			return _saveAsCommand;
		}
		set
		{
			_saveAsCommand = value;
			SetProperty();
		}
	}
	private ICommand _openCommand;
	public ICommand OpenCommand
	{
		get
		{
			if (_openCommand == null)
				_openCommand = new Command(Open);
			return _openCommand;
		}
		set
		{
			_openCommand = value;
			SetProperty();
		}
	}
	private ICommand _newCommand;
	public ICommand NewCommand
	{
		get
		{
			if (_newCommand == null)
				_newCommand = new Command(NewFile);
			return _newCommand;
		}
		set
		{
			_newCommand = value;
			SetProperty();
		}
	}
	private ICommand _compileCommand;
	public ICommand CompileCommand
	{
		get
		{
			if (_compileCommand == null)
				_compileCommand = new Command(Compile);
			return _compileCommand;
		}
		set
		{
			_compileCommand = value;
			SetProperty();
		}
	}
	#endregion

	private Model.TextEditor.TextEditor editor;

	public TextEditorViewModel()
	{
		editor = new Model.TextEditor.TextEditor();
		Output = "Hello, World!";

		_saveCommand = new Command(SaveFile);
		_saveAsCommand = new Command(SaveFileAs);
		_openCommand = new Command(Open);
		_newCommand = new Command(NewFile);
		_compileCommand = new Command(Compile);
	}
	public TextEditorViewModel(string filePath)
	{
		editor = new Model.TextEditor.TextEditor(OpenFile(filePath));

		_saveCommand = new Command(SaveFile);
		_saveAsCommand = new Command(SaveFileAs);
		_openCommand = new Command(Open);
		_newCommand = new Command(NewFile);
		_compileCommand = new Command(Compile);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetProperty([CallerMemberName] string? propertyName = null)
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
			_path = path;
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
				if (editor.GetUnderCursor(-1) == "{")
				{
					editor.AddAtCursor('\n');
					editor.AddAtCursor('\t');
					editor.AddAtCursor('\n');
					editor.MoveLeft();
					break;
				}
				editor.AddAtCursor('\n');
				break;
			case TextEvent.EventType.Tab:
				editor.AddAtCursor('\t');
				break;
			case TextEvent.EventType.Backspace:
				editor.RemoveAtCursor();
				break;
			case TextEvent.EventType.Delete:
				if (editor.GetUnderCursor() == "")
					break;
				editor.MoveRight();
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
			case TextEvent.EventType.End:
				editor.MoveRight(editor.GetLineLength(editor.Line) - editor.CursorPositionInLine);
				break;
			case TextEvent.EventType.PageDown:
				editor.SetCursorPos(editor.Buffer.Length);
				break;
			case TextEvent.EventType.Home:
				editor.MoveLeft(editor.CursorPositionInLine);
				break;
			case TextEvent.EventType.PageUp:
				editor.SetCursorPos(0);
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
				editor.AddAtCursor('{');
				editor.AddAtCursor('}');
				editor.MoveLeft();
				break;
			case "}":
				if (editor.GetUnderCursor() == "}")
				{
					editor.MoveRight();
					break;
				}
				editor.AddAtCursor('}');
				break;
			case "(":
				editor.AddAtCursor('(');
				editor.AddAtCursor(')');
				editor.MoveLeft();
				break;
			case ")":
				if (editor.GetUnderCursor() == ")")
				{
					editor.MoveRight();
					break;
				}
				editor.AddAtCursor(')');
				break;
			default:
				editor.AddAtCursor(text);
				break;
		}
	}

	private void SaveFile()
	{
		if (string.IsNullOrEmpty(_path))
		{
			SaveFileAs();
		}
		else
		{
			File.WriteAllText(_path, Text);
		}
	}
	private void SaveFileAs()
	{
		// Configure open file dialog box
		var dialog = new Microsoft.Win32.SaveFileDialog();
		dialog.FileName = "program"; // Default file name
		dialog.DefaultExt = ".mos"; // Default file extension
		dialog.Filter = "Mossy documents (.mos)|*.mos"; // Filter files by extension

		// Show open file dialog box
		bool? result = dialog.ShowDialog();

		// Process open file dialog box results
		if (result == true)
		{
			// Open document
			string filePath = dialog.FileName;

			_path = filePath;
			SaveFile();
		}
	}

	private void Open()
	{
		// Configure open file dialog box
		var dialog = new Microsoft.Win32.OpenFileDialog();
		dialog.FileName = "program"; // Default file name
		dialog.DefaultExt = ".mos"; // Default file extension
		dialog.Filter = "Mossy documents (.mos)|*.mos"; // Filter files by extension

		// Show open file dialog box
		bool? result = dialog.ShowDialog();

		// Process open file dialog box results
		if (result == true)
		{
			// Open document
			string filePath = dialog.FileName;

			editor = new Model.TextEditor.TextEditor(OpenFile(filePath));
			_path = filePath;
		}
		Text = "";
	}
	private void NewFile()
	{
		editor = new Model.TextEditor.TextEditor();
		Text = "";
		_path = "";
	}

	public void Close()
	{

	}

	private void Compile()
	{
		if (string.IsNullOrEmpty(_path))
		{
			Output += "\n" + "File not saved! Please save file before compiling";
			return;
		}
		if (string.IsNullOrEmpty(Settings.CurrentSettings.mossyAddress))
		{
			Output += "\n" + "Mossy address not specified!";
			return;
		}
		Process.Start(new ProcessStartInfo()
		{
			FileName = Settings.CurrentSettings.mossyAddress + "/mossy.exe",
			Arguments = "build " + _path,
		});
		Output += "\n" + "Compiled!";
	}

	public class Command : ICommand
	{
		public event EventHandler? CanExecuteChanged;

		private Action _action;

		public Command(Action action)
		{
			_action = action;
		}

		public bool CanExecute(object? parameter)
		{
			return true;
		}

		public void Execute(object? parameter)
		{
			_action.Invoke();
		}
	}

}