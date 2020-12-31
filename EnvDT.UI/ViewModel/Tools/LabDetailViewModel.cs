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
        private IUnitOfWork _unitOfWork;
        private IMessageDialogService _messageDialogService;
        private ILookupDataService _lookupDataService;
        private LabWrapper _laboratory;

        public LabDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, ILookupDataService lookupDataService)
            :base(eventAggregator)
        {
            _unitOfWork = unitOfWork;
            _messageDialogService = messageDialogService;
            _lookupDataService = lookupDataService;
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

        public override void Load(Guid? laboratoryId)
        {
            var laboratory = laboratoryId.HasValue
                ? _unitOfWork.Laboratories.GetById(laboratoryId.Value)
                : CreateNewLab();

            InitializeLab(laboratoryId, laboratory);
        }

        private void InitializeLab(Guid? laboratoryId, Laboratory laboratory)
        {
            Laboratory = new LabWrapper(laboratory);
            Laboratory.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _unitOfWork.Laboratories.HasChanges();
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
            _unitOfWork.Save();
            HasChanges = _unitOfWork.Laboratories.HasChanges();
            //RaiseDetailSavedEvent(Laboratory.LabId,
                //$"{Laboratory.LabNumber} {Laboratory.LabName}");
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
            /*
            var result = _messageDialogService.ShowOkCancelDialog(
                Translator["EnvDT.UI.Properties.Strings.LabDetailVM_DialogTitle_ConfirmDeletion"],
                string.Format(Translator["EnvDT.UI.Properties.Strings.LabDetailVM_DialogMsg_ConfirmDeletion"],
                Laboratory.LabClient, Laboratory.LabName));

            if (result == MessageDialogResult.Yes)
            {
                RaiseDetailDeletedEvent(Laboratory.Model.LabId);
                SetPropertyValueToNull(this, "LabReportViewModel");
                _unitOfWork.Laboratories.Delete(Laboratory.Model);
                _unitOfWork.Save();
            }*/
        }

        protected override bool OnDeleteCanExecute()
        {
            return Laboratory != null && Laboratory.LaboratoryId != Guid.Empty 
                && _unitOfWork.Laboratories.GetById(Laboratory.LaboratoryId) != null;
        }

        private Laboratory CreateNewLab()
        {
            var laboratory = new Laboratory();
            //_unitOfWork.Laboratories.Create(laboratory);
            return laboratory;
        }
    }
}
