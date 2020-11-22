using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public class MissingParamDialogViewModel : DetailViewModelBase, IMissingParamDialogViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnitOfWork _unitOfWork;
        private ObservableCollection<MissingParamNameWrapper> _missingParamNames;

        public MissingParamDialogViewModel(IEventAggregator eventEggregator, IUnitOfWork unitOfWork)
            :base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            _unitOfWork = unitOfWork;
            MissingParamNames = new ObservableCollection<MissingParamNameWrapper>();
        }

        public HashSet<Guid> MissingParamIds { get; set; }

        public ObservableCollection<MissingParamNameWrapper> MissingParamNames
        {
            get { return _missingParamNames; }
            set
            {
                _missingParamNames = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? id)
        {
            throw new NotImplementedException();
        }

        public void Load(Guid labReportId, HashSet<Guid> missingParamIds)
        {
            foreach (var wrapper in MissingParamNames)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            MissingParamNames.Clear();

            foreach (Guid missingParamId in missingParamIds)
            {
                var missingParam = _unitOfWork.Parameters.GetById(missingParamId);
                var paramNameVariant = new ParamNameVariant();
                paramNameVariant.ParameterId = missingParam.ParameterId;
                var wrapper = new MissingParamNameWrapper(paramNameVariant);
                wrapper.ParamName = missingParam.ParamNameDe;
                var labReportParams = _unitOfWork.LabReportParams.GetLabReportUnknownParamNamesByLabReportId(labReportId);
                foreach (LabReportParam param in labReportParams)
                {
                    wrapper.ParamNameAliases.Add(param.LabReportParamName);
                }
                var languages = _unitOfWork.Languages.GetAll();
                foreach (Language language in languages)
                {
                    wrapper.LanguageNames.Add(language.LangAbbrev);
                }

                wrapper.PropertyChanged += Wrapper_PropertyChanged;

                MissingParamNames.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _unitOfWork.ParamNameVariants.HasChanges();
            }
            if (e.PropertyName == nameof(MissingParamNameWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
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
