using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Liken.ViewModel;

public interface IViewModel : INotifyPropertyChanged
{
	void SetProperty([CallerMemberName] string? propertyName = null);

	void Close();
}