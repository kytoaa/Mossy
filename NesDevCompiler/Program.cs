public class Program
{
	private static readonly Dictionary<string, Action<List<string>>> _commands = new Dictionary<string, Action<List<string>>>()
	{
		{ "build", Build }
	};

	public static void Main(string[] args)
	{
		if (args.Length > 0 && _commands.TryGetValue(args[0], out Action<List<string>>? action))
		{
			List<string> arguments = args.ToList();
			arguments.RemoveAt(0);

			action?.Invoke(arguments);
			return;
		}
		Console.WriteLine("Not a command! Use help for a list of commands!");
	}

	private static void Build(List<string> args)
	{
		string path = args[0];
		if (!Path.Exists(path))
		{
			Console.WriteLine("File does not exist! Please enter a valid path");
			return;
		}

		string mossyCode = File.ReadAllText(args[0]);

		var compiler = new Mossy.Compiler("6502");

		if (compiler.TryCompile(mossyCode, out var result))
		{
			Console.WriteLine("Compilation Successful");
			string newPath = Path.ChangeExtension(path, ".s");
			File.WriteAllText(newPath, result);
		}
		else
		{
			var colour = Console.BackgroundColor;
			Console.BackgroundColor = ConsoleColor.Red;
			Console.WriteLine(result);
			Console.BackgroundColor = colour;
		}
	}
}