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

        WeakReferenceMessenger.Default.Register<HideKeyboardMessage>(this, async (r, m) =>
        {
            await App.Current.Dispatcher.DispatchAsync(async () =>
            {
				await messageEntry.HideKeyboardAsync();
            });
        });

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
