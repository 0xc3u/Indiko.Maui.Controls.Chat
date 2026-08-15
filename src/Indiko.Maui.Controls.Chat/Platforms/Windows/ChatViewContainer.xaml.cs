using Microsoft.UI.Xaml.Controls;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// The Windows chat view: a virtualized <see cref="ListView"/> (newest message at the bottom)
/// with the floating scroll-to-bottom button overlaid. Owned by <see cref="ChatViewHandler"/>.
/// </summary>
public partial class ChatViewContainer : Grid
{
    internal ChatViewHandler Handler { get; set; }

    internal ListView MessagesList => ChatList;

    internal ScrollToBottomButton Fab => ChatFab;

    public ChatViewContainer()
    {
        InitializeComponent();
        ChatFab.Click += (s, e) => Handler?.OnFabTapped();
    }

    private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        => Handler?.OnContainerContentChanging(sender, args);
}
