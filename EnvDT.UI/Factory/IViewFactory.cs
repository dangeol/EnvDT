using EnvDT.UI.ViewModel;
using System.Windows.Controls;

namespace EnvDT.UI.Factory
{
    public interface IViewFactory
    {
        public void Register<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : UserControl;

        public UserControl Resolve<TViewModel>()
            where TViewModel : class, IViewModel;
    }
}
