using EnvDT.Model.Entity;
using EnvDT.UI.Wrapper;

namespace FriendOrganizer.UI.Wrapper
{
    public class LabReportWrapper : ModelWrapper<LabReport>
    {
        public LabReportWrapper(LabReport model) : base(model)
        {
        }

        public string ReportLabIdent
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }

}
