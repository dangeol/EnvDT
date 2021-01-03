﻿using EnvDT.Model.Entity;
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

        public override void Load(Guid? laboratoryId)
        {
            var laboratory = laboratoryId.HasValue
                ? UnitOfWork.Laboratories.GetById(laboratoryId.Value)
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
                //SetPropertyValueToNull(this, "ConfigXlsxDetailViewModel");
                UnitOfWork.Laboratories.Delete(Laboratory.Model);
                UnitOfWork.Save();
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
    }
}
