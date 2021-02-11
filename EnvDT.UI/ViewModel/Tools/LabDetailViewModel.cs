using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class LabDetailViewModel : DetailViewModelBase, ILabDetailViewModel
    {
        private ILookupDataService _lookupDataService;
        private Func<IConfigXlsxDetailViewModel> _configXlsxDetailVmCreator;
        private Func<IConfigCsvDetailViewModel> _ConfigCsvDetailVmCreator;
        private LabWrapper _laboratory;
        private IConfigXlsxDetailViewModel _configXlsxDetailViewModel;
        private IConfigCsvDetailViewModel _ConfigCsvDetailViewModel;
        private bool _isConfigXlsxDetailViewEnabled;
        private bool _isConfigCsvDetailViewEnabled;

        public LabDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, ILookupDataService lookupDataService,
            Func<IConfigXlsxDetailViewModel> configXlsxDetailVmCreator, 
            Func<IConfigCsvDetailViewModel> ConfigCsvDetailVmCreator)
            :base(eventAggregator, messageDialogService, unitOfWork)
        {
            _lookupDataService = lookupDataService;
            _configXlsxDetailVmCreator = configXlsxDetailVmCreator;
            _ConfigCsvDetailVmCreator = ConfigCsvDetailVmCreator;
            CreateXlsxDetailVMCommand = new DelegateCommand(OnCreateXlsxDetailVMExecute, OnCreateXlsxDetailVMCanExecute);
            CreateCsvDetailVMCommand = new DelegateCommand(OnCreateCsvDetailVMExecute, OnCreateCsvDetailVMCanExecute);
            EventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
        }

        public ICommand CreateXlsxDetailVMCommand { get; private set; }
        public ICommand CreateCsvDetailVMCommand { get; private set; }

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

        public IConfigCsvDetailViewModel ConfigCsvDetailViewModel
        {
            get { return _ConfigCsvDetailViewModel; }
            set
            {
                _ConfigCsvDetailViewModel = value;
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

        public bool IsConfigCsvDetailViewEnabled
        {
            get { return _isConfigCsvDetailViewEnabled; }
            set
            {
                _isConfigCsvDetailViewEnabled = value;
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
            var ConfigCsv = UnitOfWork.ConfigCsvs.GetByLaboratoryId(laboratoryId);

            IsConfigXlsxDetailViewEnabled = false;
            IsConfigCsvDetailViewEnabled = false;

            if (configXlsx != null)
            {
                LoadConfigXlsxDetailVm(Laboratory.LaboratoryId);               
            }
            if (ConfigCsv != null)
            {
                LoadConfigCsvDetailVm(Laboratory.LaboratoryId);
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
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (laboratoryId == null)
            {
                // Trigger the validation
                Laboratory.LabCompany = "";
                Laboratory.LabName = "";
                Laboratory.CountryId = Guid.Empty;
            }
            else
            {
                ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)CreateXlsxDetailVMCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)CreateCsvDetailVMCommand).RaiseCanExecuteChanged();
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
            ((DelegateCommand)CreateXlsxDetailVMCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CreateCsvDetailVMCommand).RaiseCanExecuteChanged();
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
                IsConfigCsvDetailViewEnabled = false;
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
            IsConfigXlsxDetailViewEnabled = true;
        }

        private void LoadConfigCsvDetailVm(Guid? laboratoryId)
        {
            ConfigCsvDetailViewModel = _ConfigCsvDetailVmCreator();
            ConfigCsvDetailViewModel.Load(laboratoryId);
            IsConfigCsvDetailViewEnabled = true;
        }

        private bool OnCreateXlsxDetailVMCanExecute()
        {
            return Laboratory != null && Laboratory.LaboratoryId != Guid.Empty && !IsConfigXlsxDetailViewEnabled
                && UnitOfWork.ConfigXlsxs.GetByLaboratoryId(Laboratory.LaboratoryId) == null;
        }

        private void OnCreateXlsxDetailVMExecute()
        {
            LoadConfigXlsxDetailVm(Laboratory.LaboratoryId);
            ((DelegateCommand)CreateXlsxDetailVMCommand).RaiseCanExecuteChanged();
        }

        private bool OnCreateCsvDetailVMCanExecute()
        {
            return Laboratory != null && Laboratory.LaboratoryId != Guid.Empty && !IsConfigCsvDetailViewEnabled
                && UnitOfWork.ConfigCsvs.GetByLaboratoryId(Laboratory.LaboratoryId) == null;
        }

        private void OnCreateCsvDetailVMExecute()
        {
            LoadConfigCsvDetailVm(Laboratory.LaboratoryId);
            ((DelegateCommand)CreateXlsxDetailVMCommand).RaiseCanExecuteChanged();
        }

        private void OnDetailDeleted(DetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(ConfigXlsxDetailViewModel):
                    SetPropertyValueToNull(this, "ConfigXlsxDetailViewModel");
                    ((DelegateCommand)CreateXlsxDetailVMCommand).RaiseCanExecuteChanged();
                    IsConfigXlsxDetailViewEnabled = false;
                    break;
                case nameof(ConfigCsvDetailViewModel):
                    SetPropertyValueToNull(this, "ConfigCsvDetailViewModel");
                    ((DelegateCommand)CreateCsvDetailVMCommand).RaiseCanExecuteChanged();
                    IsConfigCsvDetailViewEnabled = false;
                    break;
            }
        }
    }
}
