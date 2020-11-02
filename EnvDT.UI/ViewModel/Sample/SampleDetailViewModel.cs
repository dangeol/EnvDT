using EnvDT.Model.Core;
using EnvDT.Model.Core.HelperClasses;
using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using EnvDT.UI.HelperClasses;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class SampleDetailViewModel : DetailViewModelBase, ISampleDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnitOfWork _unitOfWork;
        private IEvalLabReportService _evalLabReportService;
        private string _title = "Project";

        public SampleDetailViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork,
            IEvalLabReportService evalLabReportService)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _evalLabReportService = evalLabReportService;
            Samples = new ObservableCollection<SampleWrapper>();
            SamplePublColumns = new ObservableCollection<SamplePublColumn>();
            EvalResults = new ObservableCollection<EvalResult>();
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            IsSampleTab = true;
            //For testing only:
            var samplePublColumn = new SamplePublColumn()
            {
                FieldName = "Diehlm.",
                PublicationId = new Guid("D2F66A18-2801-4911-A542-BFE369FE4773"),
                Settings = SettingsType.Default
            };
            SamplePublColumns.Add(samplePublColumn);
        }

        public ICommand EvalLabReportCommand { get; }
        public ICommand CloseDetailViewCommand { get; }

        public bool IsSampleTab { get; private set; }
        public Guid? LabReportId { get; set; }
        public ObservableCollection<SampleWrapper> Samples { get; }
        public ObservableCollection<SamplePublColumn> SamplePublColumns { get; private set; }
        public ObservableCollection<EvalResult> EvalResults { get; }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? labReportId)
        {
            SetLabReportIdAndTitle(labReportId);

            foreach (var wrapper in Samples)
            {
                wrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            Samples.Clear();

            var samples = _unitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);

            foreach (var model in samples)
            {
                var wrapper = new SampleWrapper(model);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                Samples.Add(wrapper);
            }
        }

        private void SetLabReportIdAndTitle(Guid? id)
        {
            var ReportLabIdent = _unitOfWork.LabReports.GetById((Guid)id).ReportLabIdent;
            LabReportId = id;
            Title = ReportLabIdent;
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

        private void OnEvalExecute()
        {
            // put here: _evalLabReportParams
            EvalResults.Clear();
            foreach (var sample in Samples)
            {
                // put here foreach loop through selected publications
                var evalResult = _evalLabReportService.getEvalResult(sample.SampleId, SamplePublColumns.First().PublicationId);
                EvalResults.Add(evalResult);
            }
        }

        private bool OnEvalCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnCloseDetailViewExecute()
        {
            _eventAggregator.GetEvent<DetailClosedEvent>()
                .Publish(new DetailClosedEventArgs
                {
                    Id = LabReportId,
                    ViewModelName = this.GetType().Name
                });
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
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
