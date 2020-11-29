using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
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
        private IEventAggregator _eventAggregator;
        private IUnitOfWork _unitOfWork;
        private ILookupDataService _lookupDataService;
        private ObservableCollection<SampleWrapper> _samples;

        public SampleEditDialogViewModel(IEventAggregator eventEggregator, IUnitOfWork unitOfWork,
            ILookupDataService lookupDataService)
            :base(eventEggregator)
        {
            _eventAggregator = eventEggregator;
            _unitOfWork = unitOfWork;
            _lookupDataService = lookupDataService;
            Samples = new ObservableCollection<SampleWrapper>();
        }

        public ObservableCollection<SampleWrapper> Samples
        {
            get { return _samples; }
            set
            {
                _samples = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? labReportId)
        {
            foreach (var wrapper in Samples)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }
            Samples.Clear();

            var samples = _unitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);

            foreach (Sample sample in samples)
            {
                SampleWrapper wrapper = InitializeSample(sample);

                wrapper.PropertyChanged += Wrapper_PropertyChanged;

                //Trigger validation
                //wrapper.SampleName = "";

                Samples.Add(wrapper);
            }           
        }

        private SampleWrapper InitializeSample(Sample sample)
        {
            var wrapper = new SampleWrapper(sample)
            {
                SampleName = sample.SampleName,
            };
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

            return wrapper;
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _unitOfWork.Samples.HasChanges();
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
            // This is temporary until the wrapper mechanism is fixed
            foreach (var wrapper in Samples)
            {
                var sample = _unitOfWork.Samples.GetById(wrapper.SampleId);

                if (wrapper.SelectedMedium != null)
                {
                    sample.MediumId = wrapper.SelectedMedium.LookupItemId;
                }
                if (wrapper.SelectedMediumSubType != null)
                {
                    sample.MediumSubTypeId = wrapper.SelectedMediumSubType.LookupItemId;
                }
                if (wrapper.SelectedCondition != null)
                {
                    sample.ConditionId = wrapper.SelectedCondition.LookupItemId;
                }
            }
            _unitOfWork.Save();
            HasChanges = _unitOfWork.Samples.HasChanges();
        }
    }
}
