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

        // Add an "Edit" item to the long-press context menu (demonstrates ChatInputView edit mode).
        chatView.ContextMenuItems =
        [
            new() { Name = "Copy", Tag = "copy" },
            new() { Name = "Reply", Tag = "reply" },
            new() { Name = "Edit", Tag = "edit" },
            new() { Name = "Delete", Tag = "delete", IsDestructive = true },
        ];
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
