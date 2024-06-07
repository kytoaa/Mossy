using Liken.Model.TextEditor;

namespace Liken.CLI;

public class App
{
	private TextEditor _editor;

	public void Initialize()
	{

	}

	public void Run()
	{
		while (true)
		{
			ConsoleKeyInfo key = Console.ReadKey();
			Console.WriteLine(key.KeyChar);
		}
	}
}