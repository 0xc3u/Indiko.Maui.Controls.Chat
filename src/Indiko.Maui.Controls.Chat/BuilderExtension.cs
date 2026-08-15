using Microsoft.Maui.Hosting;
#if ANDROID
using Indiko.Maui.Controls.Chat.Platforms.Android;
#endif

#if IOS
using Indiko.Maui.Controls.Chat.Platforms.iOS;
#endif

#if WINDOWS
using Indiko.Maui.Controls.Chat.Platforms.Windows;
#endif

namespace Indiko.Maui.Controls.Chat;

public static class BuilderExtension
{
    public static MauiAppBuilder UseChatView(this MauiAppBuilder builder)
    {

        builder.ConfigureMauiHandlers(handlers =>
           {
               handlers.AddHandler<ChatView, ChatViewHandler>();
           });

        return builder;
    }
}
