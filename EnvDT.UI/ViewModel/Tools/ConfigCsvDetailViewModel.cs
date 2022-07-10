using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ConfigCsvDetailViewModel : DetailViewModelBase, IConfigCsvDetailViewModel
    {

        private ConfigCsvWrapper _configCsv;
        private bool _isConfigCsvSaved;

        public ConfigCsvDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            : base(eventAggregator, messageDialogService, unitOfWork)
        {
        }

        public bool IsConfigCsvSaved
        {
            get { return _isConfigCsvSaved; }
            private set
            {
                _isConfigCsvSaved = value;
                OnPropertyChanged();
            }
        }

        public ConfigCsvWrapper ConfigCsv
        {
            get { return _configCsv; }
            private set
            {
                _configCsv = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? laboratoryId)
        {
            var configCsvFound = UnitOfWork.ConfigCsvs.GetByLaboratoryId(laboratoryId);
            var configCsv = configCsvFound != null
                ? configCsvFound
                : CreateNewConfigCsv(laboratoryId);

            InitializeConfigCsv(configCsvFound, configCsv);
        }

        private void InitializeConfigCsv(ConfigCsv configCsvFound, ConfigCsv configCsv)
        {
            ConfigCsv = new ConfigCsvWrapper(configCsv);
            ConfigCsv.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = UnitOfWork.ConfigCsvs.HasChanges();
                }
                if (e.PropertyName == nameof(ConfigCsv.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (configCsvFound == null)
            {
                IsConfigCsvSaved = false;
                // Trigger the validation
                ConfigCsv.Encoding = "";
                ConfigCsv.HeaderRow = -1;           
                ConfigCsv.DelimiterChar = "";
                ConfigCsv.DecimalSepChar = "";
                ConfigCsv.IdentWord = "";
                ConfigCsv.IdentWordRow = -1;
                ConfigCsv.ReportLabidentCol = -1;
                ConfigCsv.ReportLabidentRow = -1;
                ConfigCsv.FirstSampleValueCol = -1;
                ConfigCsv.SampleLabIdentRow = -1;
                ConfigCsv.SampleNameRow = -1;
                ConfigCsv.FirstDataRow = -1;
                ConfigCsv.ParamNameCol = -1;
                ConfigCsv.UnitNameCol = -1;
                ConfigCsv.DetectionLimitCol = -1;
                ConfigCsv.MethodCol = -1;
            }
            else
            {
                IsConfigCsvSaved = true;
            }
        }

        protected override void OnSaveExecute()
        {
            UnitOfWork.Save();
            HasChanges = UnitOfWork.ConfigCsvs.HasChanges();
            IsConfigCsvSaved = true;
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            RaiseDetailSavedEvent(ConfigCsv.Model.ConfigCsvId,
                "ConfigCsv");
        }

        protected override bool OnSaveCanExecute()
        {
            return ConfigCsv != null
                && !ConfigCsv.HasErrors
                && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            if (IsConfigCsvSaved)
            {
                var result = MessageDialogService.ShowOkCancelDialog(
                    Translator["EnvDT.UI.Properties.Strings.ConfigDetailVM_DialogTitle_ConfirmDeletion"],
                    string.Format(Translator["EnvDT.UI.Properties.Strings.ConfigDetailVM_DialogMsg_ConfirmDeletion"]));

                if (result == MessageDialogResult.OK)
                {
                    RaiseDetailDeletedEvent(ConfigCsv.Model.ConfigCsvId);
                    UnitOfWork.ConfigCsvs.Delete(ConfigCsv.Model);
                    UnitOfWork.Save();
                }
            }
            else
            {
                RaiseDetailDeletedEvent(ConfigCsv.Model.ConfigCsvId);
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return ConfigCsv != null && ConfigCsv.ConfigCsvId != Guid.Empty;
        }

        private ConfigCsv CreateNewConfigCsv(Guid? laboratoryId)
        {
            var configCsv = new ConfigCsv();
            configCsv.LaboratoryId = (Guid)laboratoryId;
            UnitOfWork.ConfigCsvs.Create(configCsv);
            return configCsv;
        }
    }
}
