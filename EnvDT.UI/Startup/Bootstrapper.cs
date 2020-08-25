using Autofac;
using EnvDT.DataAccess;
using EnvDT.UI.Data;
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
            
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<PublicationDataService>().As<IPublicationDataService>();
            
            return builder.Build();
        }
    }
}
