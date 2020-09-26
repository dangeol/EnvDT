using Autofac;
using EnvDT.DataAccess;
using EnvDT.DataAccess.DataService;
using EnvDT.DataAccess.Repository;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Service;
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

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<MainViewModelDesignTime>().AsSelf();
            builder.RegisterType<ProjectViewModel>().As<IProjectViewModel>();
            builder.RegisterType<NavItemViewModel>().AsSelf();
            builder.RegisterType<EvalViewModel>().As<IEvalViewModel>();
            builder.RegisterType<ProjectDetailViewModel>().As<IProjectDetailViewModel>();

            builder.RegisterType<OpenLabReportService>().As<IOpenLabReportService>();
            builder.RegisterType<ImportLabReportService>().As<IImportLabReportService>();
            builder.RegisterType<EvalLabReportService>().As<IEvalLabReportService>();

            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<ProjectRepository>().As<IProjectRepository>();

            return builder.Build();
        }
    }
}
