using System;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface ILabReportViewModel
    {
        public ObservableCollection<NavItemViewModel> LabReports { get; }
        public void Load(Guid? projectId);
    }
}
