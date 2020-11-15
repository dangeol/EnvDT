using Autofac;
using EnvDT.UI.ViewModel;

namespace EnvDT.UI.Startup
{
    public class Bootstrapper
	{
		public Bootstrapper()
		{
		}

		public void Run()
		{
			var builder = new ContainerBuilder();
			ConfigureContainer(builder);
			var container = builder.Build();
			ConfigureApplication(container);
		}

		private void ConfigureApplication(IContainer container)
		{
			var mainWindow = container.Resolve<MainWindow>();
			mainWindow.DataContext = container.Resolve<MainViewModel>();
			mainWindow.Show();
		}

		private void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule<GeneralModule>();
			builder.RegisterModule<ViewModelModule>();
		}
	}
}
