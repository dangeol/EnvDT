using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using EnvDT.UI.Wrapper;
using Prism.Commands;
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
        private ILookupDataService _lookupDataService;
        private ObservableCollection<MissingParamNameWrapper> _missingParamNames;

        public MissingParamDialogViewModel(IEventAggregator eventEggregator, IUnitOfWork unitOfWork,
            ILookupDataService lookupDataService)
            :base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            _unitOfWork = unitOfWork;
            _lookupDataService = lookupDataService;
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
                var wrapper = new MissingParamNameWrapper(paramNameVariant);
                

                wrapper.ParamName = missingParam.ParamNameDe;
                wrapper.ParameterId = missingParam.ParameterId;
                var labReportParams = _lookupDataService.GetLabReportUnknownParamNamesLookupByLabReportId(labReportId);
                foreach (LookupItem param in labReportParams)
                {
                    wrapper.ParamNameAliases.Add(param);
                }
                var languages = _lookupDataService.GetAllLanguagesLookup();
                foreach (LookupItem language in languages)
                {
                    wrapper.LanguageNames.Add(language);
                }

                wrapper.PropertyChanged += Wrapper_PropertyChanged;

                //Trigger validation
                //wrapper.ParamNameAlias = "";
                //wrapper.LanguageId = Guid.Empty;

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
            return MissingParamNames.All(mpn => !mpn.HasErrors);
        }

        protected override void OnSaveExecute()
        {
            foreach (var wrapper in MissingParamNames)
            {
                var labReportParamId = wrapper.SelectedParameter.LookupItemId;
                if (labReportParamId != Guid.Empty)
                {
                    _unitOfWork.ParamNameVariants.Create(wrapper.Model);
                    var labReportParam = _unitOfWork.LabReportParams.GetById(labReportParamId);
                    labReportParam.ParameterId = wrapper.ParameterId;
                }
            }
            _unitOfWork.Save();
            HasChanges = _unitOfWork.ParamNameVariants.HasChanges();
        }
    }
}
