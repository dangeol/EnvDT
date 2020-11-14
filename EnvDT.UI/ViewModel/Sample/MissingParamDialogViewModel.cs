using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnvDT.UI.ViewModel
{
    public class MissingParamDialogViewModel : DetailViewModelBase, IMissingParamDialogViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnitOfWork _unitOfWork;
        private IMessageDialogService _messageDialogService;
        private ObservableCollection<MissingParamNameWrapper> _missingParamNames;
        //Only for testing
        private string _missingParamName;

        public MissingParamDialogViewModel(IEventAggregator eventEggregator, IUnitOfWork unitOfWork,
            IMessageDialogService messageDialogService)
            :base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            _unitOfWork = unitOfWork;
            _messageDialogService = messageDialogService;
            //Title = "Notification";
            MissingParamNames = new ObservableCollection<MissingParamNameWrapper>();
            MissingParamName = "Test";
        }

        public HashSet<Guid> MissingParamIds { get; set; }

        public ObservableCollection<MissingParamNameWrapper> MissingParamNames
        {
            get { return _missingParamNames; }
            private set
            {
                _missingParamNames = value;
                OnPropertyChanged();
            }
        }

        //Only for testing purpose
        public string MissingParamName
        {
            get { return _missingParamName; }
            set
            {
                _missingParamName = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? id)
        {
            throw new NotImplementedException();
        }

        public void Load(HashSet<Guid> missingParamIds)
        {
            var firstMissingParamId = missingParamIds.First();
            MissingParamName = _unitOfWork.Parameters.GetById(firstMissingParamId).ParamNameDe;
            var result = _messageDialogService.ShowMissingParamDialog("Missing parameters");
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
