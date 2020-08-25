using System.Windows.Input;
using System;

namespace EnvDT.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMember;

        public NavigationItemViewModel(Guid id, string displayMember)
        {
            LookupItemId = id;
            DisplayMember = displayMember;
        }

        public Guid LookupItemId { get; }

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDetailViewCommand { get; }
    }
}
