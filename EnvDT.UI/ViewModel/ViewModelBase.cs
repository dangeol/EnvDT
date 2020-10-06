using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnvDT.UI.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase()
        {
            
        }

        public static ViewModelBase _selectedTabbedViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static ObservableCollection<ViewModelBase> TabbedViewModels { get; protected set; }

        public static ViewModelBase SelectedTabbedViewModel
        {
            get { return _selectedTabbedViewModel; }
            set
            {
                _selectedTabbedViewModel = value;
            }
        }
    }
}
