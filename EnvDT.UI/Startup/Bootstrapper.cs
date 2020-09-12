using Autofac;
using EnvDT.DataAccess;
using EnvDT.UI.Data.Repository;
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

            builder.RegisterType<EnvDTDbContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<ProjectMainViewModel>().As<IProjectMainViewModel>();
            builder.RegisterType<ProjectItemViewModel>().AsSelf();
            builder.RegisterType<ProjectEditViewModel>().As<IProjectEditViewModel>();

            builder.RegisterType<ProjectRepository>().As<IProjectRepository>();

            builder.RegisterType<OpenLabReportService>().As<IOpenLabReportService>();
            builder.RegisterType<ImportLabReportService>().As<IImportLabReportService>();
            builder.RegisterType<EvalLabReportService>().As<IEvalLabReportService>();

            return builder.Build();
        }
    }
}
