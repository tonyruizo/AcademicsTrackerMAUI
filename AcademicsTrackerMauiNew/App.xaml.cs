using AcademicsTrackerMauiNew.Services;
using AcademicsTrackerMauiNew.Views;

namespace AcademicsTrackerMauiNew
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; internal set; } = default!;
        public App()
        {
            InitializeComponent();

            var homePage = ActivatorUtilities.CreateInstance<HomePage>(ServiceProvider!);
            MainPage = new NavigationPage(homePage);

            _ = Task.Run(async () =>
            {
                try
                {
                    var db = ServiceProvider.GetRequiredService<DatabaseService>();
                    await db.InitializeAndSeedAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DB init error: " + ex);
                }
            });
        }
    }
}
