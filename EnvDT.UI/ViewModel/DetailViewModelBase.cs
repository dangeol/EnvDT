using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Reflection;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{   
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;
        protected readonly IEventAggregator EventAggregator;

        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        }

        public abstract void Load(Guid? id);
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        
        public bool HasChanges
        {
            get { return _hasChanges; }
            set 
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        protected abstract void OnSaveExecute();
        protected abstract bool OnSaveCanExecute();
        protected abstract void OnDeleteExecute();
        protected abstract bool OnDeleteCanExecute();

        protected void SetPropertyValueToNull(object instance, string property)
        {
            PropertyInfo prop = instance.GetType().GetProperty(property);
            prop.SetValue(instance, null, null);
        }

        protected virtual void RaiseDetailSavedEvent(Guid modelId, string displayMember)
        {
            EventAggregator.GetEvent<DetailSavedEvent>()
                .Publish(
                new DetailSavedEventArgs
                {
                    Id = modelId,
                    DisplayMember = displayMember,
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailDeletedEvent(Guid modelId)
        {
            EventAggregator.GetEvent<DetailDeletedEvent>()
                .Publish(
                new DetailDeletedEventArgs
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }
    }
}
