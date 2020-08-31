using Autofac;
using EnvDT.DataAccess;
using EnvDT.UI.Data.Lookups;
using EnvDT.UI.Data.Repositories;
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
            builder.RegisterType<PublicationDetailViewModel>().As<IPublicationDetailViewModel>();
            builder.RegisterType<GetLabReportViewModel>().As<IGetLabReportViewModel>();

            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<PublicationRepository>().As<IPublicationRepository>();
            builder.RegisterType<OpenLabReportService>().As<IOpenLabReportService>();
            builder.RegisterType<ImportLabReportService>().As<IImportLabReportService>();
            builder.RegisterType<EvalLabReportService>().As<IEvalLabReportService>();

            return builder.Build();
        }
    }
}
