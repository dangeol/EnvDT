using Autofac;
using EnvDT.UI.Startup;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModelDesignTime : MainViewModel
    {
        public MainViewModelDesignTime()
            : base(ContainerProvider.Container.Resolve<MainTabViewModel>(), 
                  ContainerProvider.Container.Resolve<ISettingsDetailViewModel>())
        {
        }
    }
}
