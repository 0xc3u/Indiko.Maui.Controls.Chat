using CommunityToolkit.Mvvm.ComponentModel;
using Indiko.Maui.Controls.Chat.Sample.Interfaces;

namespace Indiko.Maui.Controls.Chat.Sample.ViewModels;

public abstract partial class BaseViewModel : ObservableObject, IViewModel
{
	[ObservableProperty]
	bool isBusy;

	public abstract Task OnAppearing(object param);
}
