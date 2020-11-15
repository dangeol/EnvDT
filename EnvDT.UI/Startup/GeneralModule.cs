using Autofac;
using EnvDT.DataAccess;
using EnvDT.DataAccess.DataService;
using EnvDT.DataAccess.Repository;
using EnvDT.Model.Core;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Service;
using EnvDT.UI.ViewModel;
using Prism.Events;

namespace EnvDT.UI.Startup
{
    public class GeneralModule : Module
    {
		protected override void Load(ContainerBuilder builder)
		{
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<EnvDTDbContext>().AsSelf();
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<Tab>().As<ITab>().SingleInstance();
            builder.RegisterType<ProjectRepository>().As<IProjectRepository>();
            builder.RegisterType<LabReportRepository>().As<ILabReportRepository>();
            builder.RegisterType<SampleRepository>().As<ISampleRepository>();
            builder.RegisterType<SampleValueRepository>().As<ISampleValueRepository>();
            builder.RegisterType<OpenLabReportService>().As<IOpenLabReportService>();
            builder.RegisterType<ImportLabReportService>().As<IImportLabReportService>();
            builder.RegisterType<EvalLabReportService>().As<IEvalLabReportService>();
            builder.RegisterType<LabReportPreCheck>().As<ILabReportPreCheck>();
            builder.RegisterType<EvalCalc>().As<IEvalCalc>();
            builder.RegisterType<ReadFileHelper>().As<IReadFileHelper>();
        }
	}
}
