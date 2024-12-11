using CommunityToolkit.Maui;
using Indiko.Maui.Controls.Chat.Sample.ViewModels;

namespace Indiko.Maui.Controls.Chat.Sample;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseChatView()
            .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

            fonts.AddFont("Font-Awesome-6-Brands-Regular-400.otf", "FontAwesomeBrandsRegular");
            fonts.AddFont("Font-Awesome-6-Free-Regular-400.otf", "FontAwesomeRegular");
            fonts.AddFont("Font-Awesome-6-Free-Solid-900.otf", "FontAwesomeSolid");
        });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

        return builder.Build();
    }
}