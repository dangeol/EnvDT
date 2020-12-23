using EnvDT.UI.Settings.Localization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnvDT.UI.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected TranslationSource Translator = TranslationSource.Instance;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
