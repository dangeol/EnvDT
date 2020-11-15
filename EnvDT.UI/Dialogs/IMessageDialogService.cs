using EnvDT.UI.ViewModel;

namespace EnvDT.UI.Dialogs
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowYesNoDialog(string title, string message);
        MessageDialogResult ShowOkDialog(string title, string message);
        MessageDialogResult ShowOkCancelDialog(string title, string message);
        MessageDialogResult ShowMissingParamDialog(string title, 
            IMissingParamDialogViewModel missingParamDetailVM);
    }
    public enum MessageDialogResult
    {
        Yes,
        No,
        OK,
        Cancel
    }
}
