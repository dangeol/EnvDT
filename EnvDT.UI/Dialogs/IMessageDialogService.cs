namespace EnvDT.UI.Dialogs
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowYesNoDialog(string title, string message);
        MessageDialogResult ShowOkDialog(string title, string message);
    }
    public enum MessageDialogResult
    {
        Yes,
        No,
        OK
    }
}
