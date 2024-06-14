using System.IO;
using System.Text.Json;
using System.Windows.Media;

namespace Liken.Model;

public static class Settings
{
    private static EditorSettings _editorSettings = new EditorSettings();
    public static EditorSettings CurrentSettings => _editorSettings;

    public static string SaveAddress => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "config.json");

    public static void Init()
    {
        _editorSettings = GetSavedSettings();
    }

    public static void SetCC65Address(string address)
    {
        _editorSettings.cc65Address = address;
    }
    public static void SetMossyAddress(string address)
    {
        _editorSettings.mossyAddress = address;
    }

    public static void SetColour(int index, string colour)
    {
        _editorSettings.colours[index] = (Color)ColorConverter.ConvertFromString(colour);
    }
    public static string GetColour(int index)
    {
        return new ColorConverter().ConvertToString(_editorSettings.colours[index]).Replace("#", "");
    }

    public static void SaveSettings()
    {
        JsonSerializerOptions options = new JsonSerializerOptions() { IncludeFields = true, WriteIndented = true };
        string savedPrefs = JsonSerializer.Serialize(CurrentSettings, options);

        if (!Path.Exists(SaveAddress))
        {
            Console.WriteLine($"Config directory does not exist, creating it one at {Path.GetDirectoryName(SaveAddress)}");
            Directory.CreateDirectory(Path.GetDirectoryName(SaveAddress));
        }

        File.WriteAllText(SaveAddress, savedPrefs);
    }
    public static EditorSettings GetSavedSettings()
    {
        EditorSettings settings;

        try
        {
            string savedPrefs = File.ReadAllText(SaveAddress);
            settings = JsonSerializer.Deserialize<EditorSettings>(savedPrefs);
        }
        catch (Exception)
        {
            return new EditorSettings();
        }
        return settings;
    }

    public struct EditorSettings
    {
        /// <summary>
        /// The root cc65 directory.
        /// </summary>
        public string cc65Address;
        /// <summary>
        /// The root mossy directory.
        /// </summary>
        public string mossyAddress;

        public Color[] colours;

        public EditorSettings()
        {
            cc65Address = string.Empty;
            mossyAddress = AppDomain.CurrentDomain.BaseDirectory;
            colours = new Color[6];
        }

        public struct Colour
        {
            byte r, g, b;

            public static explicit operator Color(Colour c)
            {
                return Color.FromRgb(c.r, c.g, c.b);
            }
        }
    }
}
