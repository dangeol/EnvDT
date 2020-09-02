using Autofac;
using EnvDT.DataAccess;
using EnvDT.UI.Data.Services;
using EnvDT.UI.ViewModel;
using Prism.Events;

namespace EnvDT.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<EnvDTDbContext>().AsSelf();
           
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();

            builder.RegisterType<OpenLabReportService>().As<IOpenLabReportService>();
            builder.RegisterType<ImportLabReportService>().As<IImportLabReportService>();
            builder.RegisterType<EvalLabReportService>().As<IEvalLabReportService>();

            return builder.Build();
        }
    }
}
