using Liken.Model;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Liken.ViewModel;

public class SettingsViewModel : IViewModel
{
	public SettingsViewModel()
	{
		Settings.Init();
	}

	public string BackgroundColour
	{
		get => Settings.GetColour(0);
		set
		{
			Settings.SetColour(0, value);
			SetProperty();
		}
	}
	public string HighlightColour
	{
		get => Settings.GetColour(1);
		set
		{
			Settings.SetColour(1, value);
			SetProperty();
		}
	}
	public string BaseTextColour
	{
		get => Settings.GetColour(2);
		set
		{
			Settings.SetColour(2, value);
			SetProperty();
		}
	}
	public string KeywordColour
	{
		get => Settings.GetColour(3);
		set
		{
			Settings.SetColour(3, value);
			SetProperty();
		}
	}
	public string FunctionColour
	{
		get => Settings.GetColour(4);
		set
		{
			Settings.SetColour(4, value);
			SetProperty();
		}
	}

	public string MossyAddress
	{
		get => Settings.CurrentSettings.mossyAddress;
		set
		{
			if (!Path.Exists(value))
				return;
			Settings.SetMossyAddress(value);
			SetProperty();
		}
	}
	public string CC65Address
	{
		get => Settings.CurrentSettings.cc65Address;
		set
		{
			if (!Path.Exists(value))
				return;
			Settings.SetCC65Address(value);
			SetProperty();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public void SetProperty([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void Close()
	{
		Settings.SaveSettings();
	}
}