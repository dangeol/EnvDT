using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ConfigXlsxDetailViewModel : DetailViewModelBase, IConfigXlsxDetailViewModel
    {

        private ConfigXlsxWrapper _configXlsx;
        private bool _isConfigXlsxSaved;

        public ConfigXlsxDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            :base(eventAggregator, messageDialogService, unitOfWork)
        {
        }

        public bool IsConfigXlsxSaved
        {
            get { return _isConfigXlsxSaved; }
            private set
            {
                _isConfigXlsxSaved = value;
                OnPropertyChanged();
            }
        }

        public ConfigXlsxWrapper ConfigXlsx
        {
            get { return _configXlsx; }
            private set
            {
                _configXlsx = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? laboratoryId)
        {
            var configXlsxFound = UnitOfWork.ConfigXlsxs.GetByLaboratoryId(laboratoryId);
            var configXlsx = configXlsxFound != null
                ? configXlsxFound
                : CreateNewConfigXlsx(laboratoryId);

            InitializeConfigXlsx(configXlsxFound, configXlsx);
        }

        private void InitializeConfigXlsx(ConfigXlsx configXlsxFound, ConfigXlsx configXlsx)
        {
            ConfigXlsx = new ConfigXlsxWrapper(configXlsx);
            ConfigXlsx.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = UnitOfWork.ConfigXlsxs.HasChanges();
                }
                if (e.PropertyName == nameof(ConfigXlsx.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (configXlsxFound == null)
            {
                IsConfigXlsxSaved = false;
                // Trigger the validation
                ConfigXlsx.WorksheetName = "";
                ConfigXlsx.IdentWord = "";
                ConfigXlsx.IdentWordCol = -1;
                ConfigXlsx.IdentWordRow = -1;
                ConfigXlsx.ReportLabidentCol = -1;
                ConfigXlsx.ReportLabidentRow = -1;
                ConfigXlsx.FirstSampleValueCol = -1;
                ConfigXlsx.SampleLabIdentRow = -1;
                ConfigXlsx.SampleNameRow = -1;
                ConfigXlsx.FirstDataRow = -1;
                ConfigXlsx.ParamNameCol = -1;
                ConfigXlsx.UnitNameCol = -1;
                ConfigXlsx.DetectionLimitCol = -1;
                ConfigXlsx.MethodCol = -1;
            }
            else
            {
                IsConfigXlsxSaved = true;
            }
        }

        protected override void OnSaveExecute()
        {
            UnitOfWork.Save();
            HasChanges = UnitOfWork.ConfigXlsxs.HasChanges();
            //RaiseDetailSavedEvent(ConfigXlsx.ConfigXlsxId,
            //$"{ConfigXlsx.ConfigXlsxNumber} {ConfigXlsx.ConfigXlsxName}");
            IsConfigXlsxSaved = true;
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

        protected override bool OnSaveCanExecute()
        {
            return ConfigXlsx != null 
                && !ConfigXlsx.HasErrors 
                && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            if (IsConfigXlsxSaved)
            {
                var result = MessageDialogService.ShowOkCancelDialog(
                    Translator["EnvDT.UI.Properties.Strings.ConfigDetailVM_DialogTitle_ConfirmDeletion"],
                    string.Format(Translator["EnvDT.UI.Properties.Strings.ConfigDetailVM_DialogMsg_ConfirmDeletion"]));

                if (result == MessageDialogResult.OK)
                {
                    RaiseDetailDeletedEvent(ConfigXlsx.Model.ConfigXlsxId);
                    UnitOfWork.ConfigXlsxs.Delete(ConfigXlsx.Model);
                    UnitOfWork.Save();
                }
            }
            else
            {
                RaiseDetailDeletedEvent(ConfigXlsx.Model.ConfigXlsxId);
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return ConfigXlsx != null && ConfigXlsx.ConfigXlsxId != Guid.Empty;
        }

        private ConfigXlsx CreateNewConfigXlsx(Guid? laboratoryId)
        {
            var configXlsx = new ConfigXlsx();
            configXlsx.LaboratoryId = (Guid)laboratoryId;
            UnitOfWork.ConfigXlsxs.Create(configXlsx);
            return configXlsx;
        }
    }
}
