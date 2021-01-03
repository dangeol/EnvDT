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

        public ConfigXlsxDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            :base(eventAggregator, messageDialogService, unitOfWork)
        {
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

        public override void Load(Guid? configXlsxId)
        {
            var configXlsx = UnitOfWork.ConfigXlsxs.GetById(configXlsxId.Value);

            InitializeConfigXlsx(configXlsxId, configXlsx);
        }

        private void InitializeConfigXlsx(Guid? configXlsxId, ConfigXlsx configXlsx)
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
            if (configXlsxId == null)
            {
                // Trigger the validation
                ConfigXlsx.WorksheetName = "";
            }
        }

        protected override void OnSaveExecute()
        {
            UnitOfWork.Save();
            HasChanges = UnitOfWork.ConfigXlsxs.HasChanges();
            //RaiseDetailSavedEvent(ConfigXlsx.ConfigXlsxId,
                //$"{ConfigXlsx.ConfigXlsxNumber} {ConfigXlsx.ConfigXlsxName}");
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
            /*
            var result = _messageDialogService.ShowOkCancelDialog(
                Translator["EnvDT.UI.Properties.Strings.ConfigXlsxDetailVM_DialogTitle_ConfirmDeletion"],
                string.Format(Translator["EnvDT.UI.Properties.Strings.ConfigXlsxDetailVM_DialogMsg_ConfirmDeletion"],
                ConfigXlsx.ConfigXlsxClient, ConfigXlsx.ConfigXlsxName));

            if (result == MessageDialogResult.Yes)
            {
                RaiseDetailDeletedEvent(ConfigXlsx.Model.ConfigXlsxId);
                SetPropertyValueToNull(this, "LabReportViewModel");
                _unitOfWork.ConfigXlsxs.Delete(ConfigXlsx.Model);
                _unitOfWork.Save();
            }*/
        }

        protected override bool OnDeleteCanExecute()
        {
            return ConfigXlsx != null && ConfigXlsx.ConfigXlsxId != Guid.Empty 
                && UnitOfWork.ConfigXlsxs.GetById(ConfigXlsx.ConfigXlsxId) != null;
        }
    }
}
