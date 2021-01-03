using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class LabDetailViewModel : DetailViewModelBase, ILabDetailViewModel
    {
        private ILookupDataService _lookupDataService;
        private Func<IConfigXlsxDetailViewModel> _configXlsxDetailVmCreator;
        private Func<IConfigXmlDetailViewModel> _configXmlDetailVmCreator;
        private LabWrapper _laboratory;
        private IConfigXlsxDetailViewModel _configXlsxDetailViewModel;
        private IConfigXmlDetailViewModel _configXmlDetailViewModel;
        private bool _isConfigXlsxDetailViewEnabled;
        private bool _isConfigXmlDetailViewEnabled;

        public LabDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, ILookupDataService lookupDataService,
            Func<IConfigXlsxDetailViewModel> configXlsxDetailVmCreator, 
            Func<IConfigXmlDetailViewModel> configXmlDetailVmCreator)
            :base(eventAggregator, messageDialogService, unitOfWork)
        {
            _lookupDataService = lookupDataService;
            _configXlsxDetailVmCreator = configXlsxDetailVmCreator;
            _configXmlDetailVmCreator = configXmlDetailVmCreator;
        }

        public LabWrapper Laboratory
        {
            get { return _laboratory; }
            private set
            {
                _laboratory = value;
                OnPropertyChanged();
            }
        }

        public IConfigXlsxDetailViewModel ConfigXlsxDetailViewModel
        {
            get { return _configXlsxDetailViewModel; }
            set
            {
                _configXlsxDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public IConfigXmlDetailViewModel ConfigXmlDetailViewModel
        {
            get { return _configXmlDetailViewModel; }
            set
            {
                _configXmlDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsConfigXlsxDetailViewEnabled
        {
            get { return _isConfigXlsxDetailViewEnabled; }
            set
            {
                _isConfigXlsxDetailViewEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsConfigXmlDetailViewEnabled
        {
            get { return _isConfigXmlDetailViewEnabled; }
            set
            {
                _isConfigXmlDetailViewEnabled = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? laboratoryId)
        {
            var laboratory = laboratoryId.HasValue
                ? UnitOfWork.Laboratories.GetById(laboratoryId.Value)
                : CreateNewLab();

            InitializeLab(laboratoryId, laboratory);

            var configXlsx = UnitOfWork.ConfigXlsxs.GetByLaboratoryId(laboratoryId);
            var configXml = UnitOfWork.ConfigXmls.GetByLaboratoryId(laboratoryId);

            IsConfigXlsxDetailViewEnabled = false;
            IsConfigXmlDetailViewEnabled = false;

            if (configXlsx != null)
            {
                LoadConfigXlsxDetailVm(configXlsx.ConfigXlsxId);
                IsConfigXlsxDetailViewEnabled = true;
            }
            if (configXml != null)
            {
                LoadConfigXmlDetailVm(configXml.ConfigXmlId);
                IsConfigXmlDetailViewEnabled = true;
            }
        }

        private void InitializeLab(Guid? laboratoryId, Laboratory laboratory)
        {
            Laboratory = new LabWrapper(laboratory);
            Laboratory.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = UnitOfWork.Laboratories.HasChanges();
                }
                if (e.PropertyName == nameof(Laboratory.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (laboratoryId == null)
            {
                // Trigger the validation
                Laboratory.LabCompany = "";
                Laboratory.LabName = "";
                Laboratory.CountryId = Guid.Empty;
            }
            var countries = _lookupDataService.GetAllCountriesLookup();
            foreach (LookupItem country in countries)
            {
                Laboratory.Countries.Add(country);
            }
        }

        protected override void OnSaveExecute()
        {
            UnitOfWork.Save();
            HasChanges = UnitOfWork.Laboratories.HasChanges();
            RaiseDetailSavedEvent(Laboratory.LaboratoryId,
                $"{Laboratory.LabCompany} ({Laboratory.LabName})");
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

        protected override bool OnSaveCanExecute()
        {
            return Laboratory != null 
                && !Laboratory.HasErrors 
                && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            var result = MessageDialogService.ShowOkCancelDialog(
                Translator["EnvDT.UI.Properties.Strings.LabDetailVM_DialogTitle_ConfirmDeletion"],
                string.Format(Translator["EnvDT.UI.Properties.Strings.LabDetailVM_DialogMsg_ConfirmDeletion"],
                Laboratory.LabCompany, $"({Laboratory.LabName})"));

            if (result == MessageDialogResult.OK)
            {
                RaiseDetailDeletedEvent(Laboratory.Model.LaboratoryId);               
                UnitOfWork.Laboratories.Delete(Laboratory.Model);
                UnitOfWork.Save();

                IsConfigXlsxDetailViewEnabled = false;
                IsConfigXmlDetailViewEnabled = false;
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return Laboratory != null && Laboratory.LaboratoryId != Guid.Empty 
                && UnitOfWork.Laboratories.GetById(Laboratory.LaboratoryId) != null;
        }

        private Laboratory CreateNewLab()
        {
            var laboratory = new Laboratory();
            UnitOfWork.Laboratories.Create(laboratory);
            return laboratory;
        }

        private void LoadConfigXlsxDetailVm(Guid? laboratoryId)
        {
            ConfigXlsxDetailViewModel = _configXlsxDetailVmCreator();
            ConfigXlsxDetailViewModel.Load(laboratoryId);
        }

        private void LoadConfigXmlDetailVm(Guid? laboratoryId)
        {
            ConfigXmlDetailViewModel = _configXmlDetailVmCreator();
            ConfigXmlDetailViewModel.Load(laboratoryId);
        }
    }
}
