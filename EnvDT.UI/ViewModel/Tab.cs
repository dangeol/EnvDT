using EnvDT.UI.Event;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnvDT.UI.ViewModel
{
    public class Tab : ITab
    {
        public ObservableCollection<IMainTabViewModel> TabbedViewModels { get; set; }

        public IMainTabViewModel GetTabbedViewModelByEventArgs(IDetailEventArgs args)
        {
            return TabbedViewModels
                   .SingleOrDefault(vm => vm.LabReportId == args.Id
                   && vm.GetType().Name == args.ViewModelName);
        }
    }
}
