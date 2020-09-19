using Autofac;
using EnvDT.UI.Startup;
using EnvDT.UI.ViewModel;
using System;
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

        private void Application_DispatcherUnhandledException(object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unexpected error occured. Please inform the admin."
                + Environment.NewLine + e.Exception.Message, "Unexpected error");

            e.Handled = true;
        }
    }
}
