using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnvDT.UI.ViewModel
{
    public class SampleEditDialogViewModel : DetailViewModelBase, ISampleEditDialogViewModel
    {
        private ILookupDataService _lookupDataService;
        private Guid _standardGuid = new Guid("875dbf0f-5e3c-4012-9828-692e0ffa39ab");

        public SampleEditDialogViewModel(IEventAggregator eventEggregator, IMessageDialogService messageDialogService,
            IUnitOfWork unitOfWork, ILookupDataService lookupDataService)
            :base(eventEggregator, messageDialogService, unitOfWork)
        {
            _lookupDataService = lookupDataService;
            StandardGuid = _standardGuid;
            Samples = new ObservableCollection<SampleWrapper>();
        }

        public Guid StandardGuid { get; private set; }
        public ObservableCollection<SampleWrapper> Samples { get; }

        public override void Load(Guid? labReportId)
        {
            foreach (var wrapper in Samples)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }
            Samples.Clear();

            var samples = UnitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);

            foreach (Sample sample in samples)
            {
                if (sample.MediumId == null)
                { 
                    // this is to have a value there to prevent validation error 
                    // when the user clicks on "edit sample"
                    sample.MediumId = _standardGuid;
                }
                if (sample.MediumSubTypeId == null)
                {
                    sample.MediumSubTypeId = _standardGuid;
                }
                if (sample.ConditionId == null)
                {
                    sample.ConditionId = _standardGuid;
                }
                if (sample.WasteCodeEWCId == null)
                {
                    sample.WasteCodeEWCId = _standardGuid;
                }
                SampleWrapper wrapper = InitializeSample(sample);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                Samples.Add(wrapper);
            }
        }

        private SampleWrapper InitializeSample(Sample sample)
        {
            var wrapper = new SampleWrapper(sample);
            wrapper.SampleName = sample.SampleName;
            wrapper.MediumId = (Guid)sample.MediumId;
            wrapper.MediumSubTypeId = (Guid)sample.MediumSubTypeId;
            wrapper.ConditionId = (Guid)sample.ConditionId;
            wrapper.WasteCodeEWCId = (Guid)sample.WasteCodeEWCId;

            var mediumSubTypes = _lookupDataService.GetAllMediumSubTypesLookup();
            foreach (LookupItem mediumSubType in mediumSubTypes)
            {
                wrapper.MediumSubTypes.Add(mediumSubType);
            }
            var conditions = _lookupDataService.GetAllConditionsLookup();
            foreach (LookupItem condition in conditions)
            {
                wrapper.Conditions.Add(condition);
            }
            var wasteCodeEWCs = _lookupDataService.GetAllWasteCodeEWCsLookup();
            foreach (LookupItem wasteCodeEWC in wasteCodeEWCs)
            {
                wrapper.WasteCodeEWCs.Add(wasteCodeEWC);
            }

            return wrapper;
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = UnitOfWork.Samples.HasChanges();
            }
            if (e.PropertyName == nameof(SampleWrapper.HasErrors))
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
            return Samples.All(mpn => !mpn.HasErrors);
        }

        protected override void OnSaveExecute()
        {
            SetValues();
            UnitOfWork.Save();
            HasChanges = UnitOfWork.Samples.HasChanges();
            // add here event???
        }
       
        private void SetValues()
        {
            foreach (var wrapper in Samples)
            {
                var sample = UnitOfWork.Samples.GetById(wrapper.SampleId);

                if (wrapper.MediumId != null 
                    && !Guid.Equals(wrapper.MediumId, Guid.Empty) 
                    && !Guid.Equals(wrapper.MediumId, _standardGuid))
                {
                    sample.MediumId = wrapper.MediumId;
                }
                if (wrapper.MediumSubTypeId != null 
                    && !Guid.Equals(wrapper.MediumSubTypeId, Guid.Empty) 
                    && !Guid.Equals(wrapper.MediumSubTypeId, _standardGuid))
                {
                    sample.MediumSubTypeId = wrapper.MediumSubTypeId;
                }
                if (wrapper.ConditionId != null 
                    && !Guid.Equals(wrapper.ConditionId, Guid.Empty) 
                    && !Guid.Equals(wrapper.ConditionId, _standardGuid))
                {
                    sample.ConditionId = wrapper.ConditionId;
                }
                if (wrapper.WasteCodeEWCId != null
                    && !Guid.Equals(wrapper.WasteCodeEWCId, Guid.Empty)
                    && !Guid.Equals(wrapper.WasteCodeEWCId, _standardGuid))
                {
                    sample.WasteCodeEWCId = wrapper.WasteCodeEWCId;
                }
            }
        }   
    }
}
