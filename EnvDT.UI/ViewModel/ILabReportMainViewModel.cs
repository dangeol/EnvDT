using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public interface ILabReportMainViewModel
    {
        ICommand EvalLabReportCommand { get; }
        ICommand OpenLabReportCommand { get; }
    }
}