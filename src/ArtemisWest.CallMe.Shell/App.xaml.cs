using System.Windows;

namespace ArtemisWest.CallMe.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Bootstrapper bootstrapper = new Bootstrapper();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            bootstrapper.Dispose();
            base.OnExit(e);
        }
    }
}
