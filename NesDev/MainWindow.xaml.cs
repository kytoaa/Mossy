using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace NesDev
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _filePath = "";

		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonCompile_Click(object sender, RoutedEventArgs e)
		{
			//new NesDevCompiler.Parser.AbstractSyntaxTree.ExpressionParser().Parse(new NesDevCompiler.Lexer.Lexer(new NesDevCompiler.CharacterStream.CharacterStream("(1 + 3) - 2;")));

			if (string.IsNullOrEmpty(_filePath))
				return;
			string path = Path.ChangeExtension(_filePath, ".s");

			string text = "";

			// TODO Compile here

			NesDevCompiler.Lexer.ILexer lexer = new NesDevCompiler.Lexer.Lexer(new NesDevCompiler.CharacterStream.CharacterStream(txtCode.Text));
			try
			{
				NesDevCompiler.Parser.AbstractSyntaxTree.Node node = new NesDevCompiler.Parser.Parser().Parse(lexer);
				Debug.WriteLine(node);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
/*			while (!lexer.End())
			{
				NesDevCompiler.Lexer.Token next = lexer.Next();

				text += next.Type;
				text += " ";
				text += next.Value;
				text += "\n";

				if (lexer.End())
					break;
			}

			File.WriteAllText(path, text);*/
		}

		private void ButtonFileOpen_Click(object sender, RoutedEventArgs e)
		{
			// Configure open file dialog box
			var dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.FileName = "program"; // Default file name
			dialog.DefaultExt = ".nesdev"; // Default file extension
			dialog.Filter = "NesDev documents (.nesdev)|*.nesdev"; // Filter files by extension

			// Show open file dialog box
			bool? result = dialog.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				string filename = dialog.FileName;
				_filePath = filename;
				txtFileName.Content = filename;
				
				txtCode.Text = File.ReadAllText(filename);
			}
		}

		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(_filePath))
			{
				SaveAs();
			}
			else
			{
				File.WriteAllText(_filePath, txtCode.Text);
			}
		}

		private void SaveAs()
		{
			// Configure save file dialog box
			var dialogue = new Microsoft.Win32.SaveFileDialog();
			dialogue.FileName = "Document"; // Default file name
			dialogue.DefaultExt = ".nesdev"; // Default file extension
			dialogue.Filter = "NesDev documents (.nesdev)|*.nesdev"; // Filter files by extension

			// Show save file dialog box
			bool? result = dialogue.ShowDialog();

			// Process save file dialog box results
			if (result == true)
			{
				// Save document
				string filename = dialogue.FileName;

				File.WriteAllText(filename, txtCode.Text);
				_filePath = filename;
				txtFileName.Content = filename;
			}
		}
	}
}