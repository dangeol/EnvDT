using EnvDT.UI.Wrapper;
using System;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface ISampleEditDialogViewModel
    {
        public void Load(Guid? labReportId);
        public ObservableCollection<SampleWrapper> Samples { get; }
    }
}
