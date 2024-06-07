using System.Runtime.InteropServices;

public class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		Console.WriteLine("Hello, World!");
		if (args.Length > 0)
		{
			Console.WriteLine("Hello, World!");
			Console.WriteLine("Welcome to Liken!");

			var app = new Liken.CLI.App();
			app.Initialize();
			app.Run();
		}
		else
		{
			HideConsoleWindow();

			var app = new Liken.App();
			app.InitializeComponent();
			app.Run();
		}

	}

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern bool AllocConsole();

	[DllImport("kernel32.dll")]
	static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	const int SW_HIDE = 0;
	const int SW_SHOW = 5;

	public static void ShowConsoleWindow()
	{
		var handle = GetConsoleWindow();

		if (handle == IntPtr.Zero)
		{
			AllocConsole();
		}
		else
		{
			ShowWindow(handle, SW_SHOW);
		}
	}

	public static void HideConsoleWindow()
	{
		var handle = GetConsoleWindow();
		ShowWindow(handle, SW_HIDE);
	}
}