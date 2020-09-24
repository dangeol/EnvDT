using Autofac;
using EnvDT.UI.Startup;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModelDesignTime : MainViewModel
    {
        public MainViewModelDesignTime()
            : base(ContainerProvider.Container.Resolve<IProjectViewModel>(), 
                  ContainerProvider.Container.Resolve<IEvalViewModel>())
        {
        }
    }
}
