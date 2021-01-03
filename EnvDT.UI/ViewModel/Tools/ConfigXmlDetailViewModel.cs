using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ConfigXmlDetailViewModel : DetailViewModelBase, IConfigXmlDetailViewModel
    {

        private ConfigXmlWrapper _configXml;

        public ConfigXmlDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            :base(eventAggregator, messageDialogService, unitOfWork)
        {
        }

        public ConfigXmlWrapper ConfigXml
        {
            get { return _configXml; }
            private set
            {
                _configXml = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? configXmlId)
        {
            var configXml = UnitOfWork.ConfigXmls.GetById(configXmlId.Value);

            InitializeConfigXml(configXmlId, configXml);
        }

        private void InitializeConfigXml(Guid? configXmlId, ConfigXml configXml)
        {
            ConfigXml = new ConfigXmlWrapper(configXml);
            ConfigXml.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = UnitOfWork.ConfigXmls.HasChanges();
                }
                if (e.PropertyName == nameof(ConfigXml.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (configXmlId == null)
            {
                // Trigger the validation
                ConfigXml.RootElement = "";
            }
        }

        protected override void OnSaveExecute()
        {
            UnitOfWork.Save();
            HasChanges = UnitOfWork.ConfigXmls.HasChanges();
            //RaiseDetailSavedEvent(ConfigXml.ConfigXmlId,
                //$"{ConfigXml.ConfigXmlNumber} {ConfigXml.ConfigXmlName}");
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

        protected override bool OnSaveCanExecute()
        {
            return ConfigXml != null 
                && !ConfigXml.HasErrors 
                && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            /*
            var result = _messageDialogService.ShowOkCancelDialog(
                Translator["EnvDT.UI.Properties.Strings.ConfigXmlDetailVM_DialogTitle_ConfirmDeletion"],
                string.Format(Translator["EnvDT.UI.Properties.Strings.ConfigXmlDetailVM_DialogMsg_ConfirmDeletion"],
                ConfigXml.ConfigXmlClient, ConfigXml.ConfigXmlName));

            if (result == MessageDialogResult.Yes)
            {
                RaiseDetailDeletedEvent(ConfigXml.Model.ConfigXmlId);
                SetPropertyValueToNull(this, "LabReportViewModel");
                _unitOfWork.ConfigXmls.Delete(ConfigXml.Model);
                _unitOfWork.Save();
            }*/
        }

        protected override bool OnDeleteCanExecute()
        {
            return ConfigXml != null && ConfigXml.ConfigXmlId != Guid.Empty 
                && UnitOfWork.ConfigXmls.GetById(ConfigXml.ConfigXmlId) != null;
        }
    }
}
