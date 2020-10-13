using System;

namespace EnvDT.UI.ViewModel
{
    public interface IMainTabViewModel : IMenuViewModel
    {
        public Guid? LabReportId { get; set; }
    }
}
