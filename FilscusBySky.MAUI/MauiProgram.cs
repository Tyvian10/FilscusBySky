using FilscusBySky.MAUI.Services;
using FilscusBySky.MAUI.ViewModels;
using FilscusBySky.MAUI.Views;
using Microsoft.Extensions.Logging;

namespace FilscusBySky.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // API Service
        builder.Services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/");
        });

        // ViewModels
        builder.Services.AddTransient<RekeningenViewModel>();
        builder.Services.AddTransient<TransactiesViewModel>();

        // Views
        builder.Services.AddTransient<RekeningenPage>();
        builder.Services.AddTransient<TransactiesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}