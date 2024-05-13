namespace NesDevCompiler.Parser;

public static class Warnings
{
	private static List<string> warnings = new List<string>();

	public static IEnumerable<string> GetWarnings() => warnings;

	public static void AddWarning(string warning) => warnings.Add(warning);

	public static void ClearWarnings() => warnings.Clear();
}