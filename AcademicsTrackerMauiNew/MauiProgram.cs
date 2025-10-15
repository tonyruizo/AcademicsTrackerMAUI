using AcademicsTrackerMauiNew.Services;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace AcademicsTrackerMauiNew
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<NotificationService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            App.ServiceProvider = app.Services;
            return app; ;
        }
    }
}
