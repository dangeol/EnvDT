using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class NavItemViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private string _displayMember;

        public NavItemViewModel()
        {
        }

        public NavItemViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public NavItemViewModel(Guid lookupItemId, string displayMember,
            string detailViewModelName,
            IEventAggregator eventAggregator)
        {
            LookupItemId = lookupItemId;
            DisplayMember = displayMember;
            DetailViewModelName = detailViewModelName;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute, OnOpenDetailViewCanExecute);
            _eventAggregator = eventAggregator;
        }

        public Guid LookupItemId { get; protected set; }

        public string DisplayMember
        {
            get { return _displayMember; } 
            set 
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public string DetailViewModelName { get; protected set; }

        public virtual ICommand OpenDetailViewCommand { get; protected set; }

        protected void OnOpenDetailViewExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Publish(
                new OpenDetailViewEventArgs
                {
                    Id = LookupItemId,
                    ViewModelName = DetailViewModelName
                }); 
        }

        protected virtual bool OnOpenDetailViewCanExecute()
        {
            return LookupItemId != null;
        }
    }

    public class NavItemViewModelNull : NavItemViewModel
    {
        public new Guid? LookupItemId { get { return null; } }
        public new string DisplayMember { get { return " - "; } }
    }
}
