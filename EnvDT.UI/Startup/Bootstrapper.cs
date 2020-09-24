using Autofac;
using EnvDT.DataAccess;
using EnvDT.DataAccess.Repository;
using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
using EnvDT.UI.Data.Service;
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

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            builder.RegisterType<EnvDTDbContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<MainViewModelDesignTime>().AsSelf();
            builder.RegisterType<ProjectViewModel>().As<IProjectViewModel>();
            builder.RegisterType<NavItemViewModel>().AsSelf();
            builder.RegisterType<NavViewModelBase>().As<INavViewModel>();
            builder.RegisterType<DetailViewModelBase>().As<IDetailViewModel>();
            builder.RegisterType<ProjectDetailViewModel>().As<IProjectDetailViewModel>();
            builder.RegisterType<EvalViewModel>().As<IEvalViewModel>();

            builder.RegisterType<ProjectRepository>().As<IProjectRepository>();

            builder.RegisterType<OpenLabReportService>().As<IOpenLabReportService>();
            builder.RegisterType<ImportLabReportService>().As<IImportLabReportService>();
            builder.RegisterType<EvalLabReportService>().As<IEvalLabReportService>();

            return builder.Build();
        }
    }
}
