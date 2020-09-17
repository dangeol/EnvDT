using Autofac;
using EnvDT.UI.Startup;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModelDesignTime : MainViewModel
    {
        public MainViewModelDesignTime()
            : base(ContainerProvider.Container.Resolve<IProjectMainViewModel>(), 
                  ContainerProvider.Container.Resolve<ILabReportMainViewModel>())
        {
        }
    }
}
