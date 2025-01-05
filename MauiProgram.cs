using Microsoft.Extensions.Logging;
using CourseworkAD.Services;

namespace CourseworkAD
{
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            // Register services
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<TransactionService>();

            return builder.Build();
        }
    }
}
