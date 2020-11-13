using EnvDT.UI.Wrapper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public class MissingParamDetailViewModel : DetailViewModelBase, IMissingParamDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private ObservableCollection<MissingParamNameWrapper> _missingParamNames;

        public MissingParamDetailViewModel(IEventAggregator eventEggregator)
            : base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            MissingParamNames = new ObservableCollection<MissingParamNameWrapper>();
        }

        public ObservableCollection<MissingParamNameWrapper> MissingParamNames
        {
            get { return _missingParamNames; }
            private set
            {
                _missingParamNames = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? id)
        {
            throw new NotImplementedException();
        }

        public void Load(HashSet<Guid> missingParamIds)
        {
            //throw new NotImplementedException();
        }

        protected override bool OnDeleteCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
