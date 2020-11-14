using Autofac;
using EnvDT.UI.Factory;
using EnvDT.UI.View;
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
			var viewFactory = container.Resolve<IViewFactory>();

			RegisterViews(viewFactory);
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
			builder.RegisterModule<DependenciesModule>();
		}

		private void RegisterViews(IViewFactory viewFactory)
		{
			viewFactory.Register<MissingParamDialogViewModel, MissingParamDialogView>();
		}
	}
}
