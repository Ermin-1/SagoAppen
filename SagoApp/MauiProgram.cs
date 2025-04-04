using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;

namespace SagoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                
              .UseMauiApp<App>()
              .UseAdMob()
              .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
            AdConfig.UseTestAdUnitIds = true; // Använd testannons-ID:n under utveckling
#endif
            return builder.Build();
        }
    }
}