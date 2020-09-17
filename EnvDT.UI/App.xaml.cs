using Autofac;
using EnvDT.UI.Startup;
using EnvDT.UI.ViewModel;
using System.Windows;

namespace EnvDT.UI
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();
            ContainerProvider.Container = container;
            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.DataContext = container.Resolve<MainViewModel>();
            mainWindow.Show();
        }
    }
}
