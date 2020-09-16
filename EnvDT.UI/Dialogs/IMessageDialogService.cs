namespace EnvDT.UI.Data.Dialogs
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowYesNoDialog(string title, string message);
    }
    public enum MessageDialogResult
    {
        Yes,
        No
    }
}
