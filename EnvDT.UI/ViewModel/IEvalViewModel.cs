using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public interface IEvalViewModel
    {
        ICommand EvalLabReportCommand { get; }
        ICommand OpenLabReportCommand { get; }
    }
}