using Autofac;
using EnvDT.UI.Data;
using EnvDT.UI.ViewModel;

namespace EnvDT.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<PublicationDataService>().As<IPublicationDataService>();

            return builder.Build();
        }
    }
}
