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
        private ObservableCollection<MissingUnitNameWrapper> _missingUnitNames;
        private bool _isMissingParamNamesVisible = false;
        private bool _missingUnitNamesVisible = false;

        public MissingParamDialogViewModel(IEventAggregator eventEggregator, IUnitOfWork unitOfWork,
            ILookupDataService lookupDataService)
            :base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            _unitOfWork = unitOfWork;
            _lookupDataService = lookupDataService;
            MissingParamNames = new ObservableCollection<MissingParamNameWrapper>();
            MissingUnitNames = new ObservableCollection<MissingUnitNameWrapper>();
        }

        public HashSet<Guid> MissingParamIds { get; set; }
        public HashSet<Guid> MissingUnitIds { get; set; }

        public ObservableCollection<MissingParamNameWrapper> MissingParamNames { get; }

        public bool IsMissingParamNamesVisible
        {
            get { return _isMissingParamNamesVisible; }
            set
            {
                _isMissingParamNamesVisible = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MissingUnitNameWrapper> MissingUnitNames { get; }

        public bool MissingUnitNamesVisible
        {
            get { return _missingUnitNamesVisible; }
            set
            {
                _missingUnitNamesVisible = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? id)
        {
            throw new NotImplementedException();
        }

        public void Load(Guid labReportId, HashSet<Guid> missingParamIds, HashSet<Guid> missingUnitIds)
        {
            foreach (var wrapper in MissingParamNames)
            {
                wrapper.PropertyChanged -= ParamNameVariantsWrapper_PropertyChanged;
            }
            MissingParamNames.Clear();

            foreach (Guid missingParamId in missingParamIds)
            {
                MissingParamNameWrapper wrapper = CreateMissingParamNameWrapper(labReportId, missingParamId);
                wrapper.PropertyChanged += ParamNameVariantsWrapper_PropertyChanged;
                MissingParamNames.Add(wrapper);
            }

            foreach (var wrapper in MissingUnitNames)
            {
                wrapper.PropertyChanged -= UnitNameVariantsWrapper_PropertyChanged;
            }
            MissingUnitNames.Clear();
            
            foreach (Guid missingUnitId in missingUnitIds)
            {
                MissingUnitNameWrapper wrapper = CreateMissingUnitNameWrapper(labReportId, missingUnitId);

                wrapper.PropertyChanged += UnitNameVariantsWrapper_PropertyChanged;

                MissingUnitNames.Add(wrapper);
            }

            if (MissingParamNames.Count > 0)
            {
                IsMissingParamNamesVisible = true;
            }
            if (MissingUnitNames.Count > 0)
            {
                MissingUnitNamesVisible = true;
            }
        }

        private MissingParamNameWrapper CreateMissingParamNameWrapper(Guid labReportId, Guid missingParamId)
        {
            var missingParam = _unitOfWork.Parameters.GetById(missingParamId);
            var paramNameVariant = new ParamNameVariant();
            var wrapper = new MissingParamNameWrapper(paramNameVariant)
            {
                //Trigger validation
                ParamNameAlias = "",
                LanguageId = Guid.Empty,
                ParamName = missingParam.ParamNameDe,
                ParameterId = missingParam.ParameterId
            };
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

            return wrapper;
        }

        private MissingUnitNameWrapper CreateMissingUnitNameWrapper(Guid labReportId, Guid missingUnitId)
        {
            var missingUnit = _unitOfWork.Units.GetById(missingUnitId);
            var unitNameVariant = new UnitNameVariant();
            var wrapper = new MissingUnitNameWrapper(unitNameVariant)
            {
                //Trigger validation
                UnitNameAlias = "",
                UnitName = missingUnit.UnitName,
                UnitId = missingUnit.UnitId
            };
            var labReportUnits = _lookupDataService.GetLabReportUnknownUnitNamesLookupByLabReportId(labReportId);
            foreach (LookupItem unit in labReportUnits)
            {
                wrapper.UnitNameAliases.Add(unit);
            }

            return wrapper;
        }

        private void ParamNameVariantsWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        private void UnitNameVariantsWrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _unitOfWork.UnitNameVariants.HasChanges();
            }
            if (e.PropertyName == nameof(MissingUnitNameWrapper.HasErrors))
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
            return HasChanges 
                && MissingParamNames.All(mpn => !mpn.HasErrors)
                && MissingUnitNames.All(mpn => !mpn.HasErrors);
        }

        protected override void OnSaveExecute()
        {
            foreach (var wrapper in MissingParamNames)
            {
                if (wrapper.SelectedParameter != null)
                {
                    var labReportParamId = wrapper.SelectedParameter.LookupItemId;
                    if (labReportParamId != Guid.Empty)
                    {
                        _unitOfWork.ParamNameVariants.Create(wrapper.Model);
                        var labReportParam = _unitOfWork.LabReportParams.GetById(labReportParamId);
                        labReportParam.ParameterId = wrapper.ParameterId;
                    }
                }
            }
            foreach (var wrapper in MissingUnitNames)
            {
                if (wrapper.SelectedUnit != null)
                {
                    var labReportParamId = wrapper.SelectedUnit.LookupItemId;
                    if (labReportParamId != Guid.Empty)
                    {
                        _unitOfWork.UnitNameVariants.Create(wrapper.Model);
                        var labReportParam = _unitOfWork.LabReportParams.GetById(labReportParamId);
                        labReportParam.UnitId = wrapper.UnitId;
                    }
                }
            }
            _unitOfWork.Save();
            HasChanges = _unitOfWork.ParamNameVariants.HasChanges();
            HasChanges = _unitOfWork.UnitNameVariants.HasChanges();
        }
    }
}
