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
        private Guid _labReportId;
        private IUnitOfWork _unitOfWork;
        private ILookupDataService _lookupDataService;
        private bool _isMissingParamNamesVisible = false;
        private bool _missingUnitNamesVisible = false;
        private IEnumerable<ParamNameVariant> _paramNameVariants;
        private IEnumerable<UnitNameVariant> _unitNameVariants;

        public MissingParamDialogViewModel(IEventAggregator eventEggregator, IUnitOfWork unitOfWork,
            ILookupDataService lookupDataService)
            :base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            _unitOfWork = unitOfWork;
            _lookupDataService = lookupDataService;
            MissingParamNames = new ObservableCollection<MissingParamNameWrapper>();
            MissingUnitNames = new ObservableCollection<MissingUnitNameWrapper>();
            _paramNameVariants = unitOfWork.ParamNameVariants.GetAll();
            _unitNameVariants = unitOfWork.UnitNameVariants.GetAll();
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
            _labReportId = labReportId;

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
                ParamName = missingParam.ParamNameDe,
                ParameterId = missingParam.ParameterId
            };

            var labReportParamNames = _unitOfWork.LabReportParams.GetLabReportUnknownParamNamesByLabReportId(labReportId);

            foreach (string paramNames in labReportParamNames)
            {
                wrapper.ParamNameAliases.Add(paramNames);
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

            var labReportUnitNames = _unitOfWork.LabReportParams.GetLabReportUnknownUnitNamesByLabReportId(labReportId);
            foreach (string unitName in labReportUnitNames)
            {
                wrapper.UnitNameAliases.Add(unitName);
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
            return MissingParamNames.All(mpn => !mpn.HasErrors)
                && MissingUnitNames.All(mun => !mun.HasErrors);
        }

        protected override void OnSaveExecute()
        {
            foreach (var wrapper in MissingParamNames)
            {
                if (wrapper.ParamNameAlias != null && !string.Equals(wrapper.ParamNameAlias, "[N/A]"))
                {
                    if (!_paramNameVariants.Any(pv => string.Equals(pv.ParamNameAlias,wrapper.ParamNameAlias) 
                        && Guid.Equals(pv.ParameterId, wrapper.ParameterId)))
                    {
                        _unitOfWork.ParamNameVariants.Create(wrapper.Model);
                    }
                    var labReportParamName = wrapper.ParamNameAlias;
                    var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByLabReportIdAndParamName(_labReportId, labReportParamName);
                    foreach (var labReportParam in labReportParams)
                    { 
                        labReportParam.ParameterId = wrapper.ParameterId;
                    }
                }
            }
            foreach (var wrapper in MissingUnitNames)
            {
                if (wrapper.UnitNameAlias != null && !string.Equals(wrapper.UnitNameAlias, "[N/A]"))
                {
                    if (!_unitNameVariants.Any(uv => string.Equals(uv.UnitNameAlias, wrapper.UnitNameAlias)
                        && Guid.Equals(uv.UnitId, wrapper.UnitId)))
                    {
                        _unitOfWork.UnitNameVariants.Create(wrapper.Model);
                    }
                    var labReportUnitName = wrapper.UnitNameAlias;
                    var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByByLabReportIdAndUnitName(_labReportId, labReportUnitName);
                    foreach (var labReportParam in labReportParams)
                    {
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
