using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class ProjectItemViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private string _displayMember;

        public ProjectItemViewModel(Guid lookupItemId, string displayMember,
            IEventAggregator eventAggregator)
        {
            LookupItemId = lookupItemId;
            DisplayMember = displayMember;
            OpenProjectEditViewCommand = new DelegateCommand(OnProjectEditViewExecute);
            _eventAggregator = eventAggregator;
        }

        private void OnProjectEditViewExecute()
        {
            _eventAggregator.GetEvent<OpenProjectEditViewEvent>()
                .Publish(LookupItemId);
        }

        public Guid LookupItemId { get; private set; }
        public string DisplayMember
        {
            get { return _displayMember; } 
            set 
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenProjectEditViewCommand { get; private set; }
    }
}
