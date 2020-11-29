using EnvDT.UI.ViewModel;

namespace EnvDT.UI.Dialogs
{
    public interface IMessageDialogService
    {
        public MessageDialogResult ShowYesNoDialog(string title, string message);
        public MessageDialogResult ShowOkDialog(string title, string message);
        public MessageDialogResult ShowOkCancelDialog(string title, string message);
        public MessageDialogResult ShowMissingParamDialog(string title, 
            IMissingParamDialogViewModel missingParamDetailVM);
        public MessageDialogResult ShowSampleEditDialog(string title,
            ISampleEditDialogViewModel sampleEditDialogViewModel);
    }
    public enum MessageDialogResult
    {
        Yes,
        No,
        OK,
        Cancel
    }
}
