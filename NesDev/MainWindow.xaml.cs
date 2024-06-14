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

namespace Liken
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenNewEditor(string address = "")
		{
			if (!string.IsNullOrEmpty(address) && !Path.Exists(address))
			{
				return;
			}

			var editor = new View.TextEditorWindow(address);
			editor.Owner = this;
			editor.Show();
			this.Hide();
			editor.Closed += (_, _) => Show();
		}

		private void ButtonFileOpen_Click(object sender, RoutedEventArgs e)
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

				OpenNewEditor(filePath);
			}
		}

		private void ButtonFileNew_Click(object sender, RoutedEventArgs e)
		{
			OpenNewEditor();
		}

		private void ButtonOpenSettings_Click(object sender, RoutedEventArgs e)
		{
			var settings = new View.SettingsWindow();
			settings.Owner = this;
			settings.Show();
		}

		private void ButtonOpenDocumentation_Click(object sender, RoutedEventArgs e)
		{
			OpenAddress("http://github.com/kytoaa/Mossy");
		}

		private void ButtonOpenGithub_Click(object sender, RoutedEventArgs e)
		{
			OpenAddress("http://www.github.com/kytoaa/Mossy");
		}

		private void OpenAddress(string address)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = address,
				UseShellExecute = true,
			});
/*			await Task.Run(() => Process.Start(new ProcessStartInfo
			{
				FileName = address,
				UseShellExecute = true,
			}));*/
		}
	}
}