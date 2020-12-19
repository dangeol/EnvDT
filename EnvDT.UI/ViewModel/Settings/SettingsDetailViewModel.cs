using EnvDT.UI.Settings.Localization;
using System.Collections.ObjectModel;
using System.Globalization;

namespace EnvDT.UI.ViewModel
{
    class SettingsDetailViewModel : ViewModelBase, ISettingsDetailViewModel
    {
        private string _selectedLanguage;
        private CultureInfo _cultureInfo;

        public SettingsDetailViewModel()
        {
            Languages = new ObservableCollection<string>();
            Languages.Add("en-US");
            Languages.Add("de");
            _cultureInfo = new CultureInfo(Properties.Settings.Default.Language);
            SelectedLanguage = _cultureInfo.Name;
        }
       
        public ObservableCollection<string> Languages { get; }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                SetCultureInfo(value);
                OnPropertyChanged();
            }
        }

        private void SetCultureInfo(string value)
        {
            _selectedLanguage = value;
            _cultureInfo = new CultureInfo(_selectedLanguage);
            TranslationSource.Instance.CurrentCulture = _cultureInfo;
            Properties.Settings.Default.Language = _selectedLanguage;
            Properties.Settings.Default.Save();
        }
    }
}
