using Indiko.Maui.Controls.Chat.Sample.ViewModels;

namespace Indiko.Maui.Controls.Chat.Sample;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .UseChatView()
            .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

        return builder.Build();
    }
}