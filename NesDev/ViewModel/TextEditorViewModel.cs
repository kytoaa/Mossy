using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Linq;

namespace NesDev.ViewModel;

public class TextEditorViewModel : INotifyPropertyChanged
{
	//public event Action<int> SetCursorPos;
	private string _text = "";
	public string Text 
	{
		get => _text;
		set
		{
			_text = value;
			_text = Text.Replace("	", "    ");
			SetProperty();
		}
	}

	private Model.TextEditor.TextEditor editor = new Model.TextEditor.TextEditor();

	public TextEditorViewModel()
	{

	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetProperty([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}