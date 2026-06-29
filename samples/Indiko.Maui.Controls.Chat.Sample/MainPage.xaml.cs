using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.Messaging;
using Indiko.Maui.Controls.Chat.Sample.Messages;
using Indiko.Maui.Controls.Chat.Sample.ViewModels;

namespace Indiko.Maui.Controls.Chat.Sample;

public partial class MainPage : ContentPage
{
	MainPageViewModel mainPageViewModel;

	public MainPage(MainPageViewModel mainPageViewModel)
	{
		InitializeComponent();
		this.mainPageViewModel = mainPageViewModel;
		BindingContext = mainPageViewModel;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await mainPageViewModel.OnAppearing(null);
    }

    protected override void OnDisappearing()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        base.OnDisappearing();
    }
}
