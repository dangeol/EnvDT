using EnvDT.Model.Core;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class SampleDetailViewModel : DetailViewModelBase, ISampleDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnitOfWork _unitOfWork;
        private IEvalLabReportService _evalLabReportService;
        private DataTable _sampleTable;
        private DataTable _evalResultTable;
        private List<Guid> _publicationIds;
        private DataView _sampleDataView;
        private DataView _evalResultDataView;
        private string _title = "Project";

        public SampleDetailViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork,
            IEvalLabReportService evalLabReportService)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _evalLabReportService = evalLabReportService;
            _sampleTable = new DataTable();
            _publicationIds = new List<Guid>();
            SampleDataView = new DataView(_sampleTable);
            Samples = new List<Sample>();
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            IsSampleTab = true;
        }

        public ICommand EvalLabReportCommand { get; }
        public ICommand CloseDetailViewCommand { get; }

        public bool IsSampleTab { get; private set; }
        public Guid? LabReportId { get; set; }
        public IEnumerable<Sample> Samples { get; private set; }

        public DataView SampleDataView
        {
            get { return _sampleDataView; }
            set
            {
                _sampleDataView = value;
                OnPropertyChanged();
            }
        }

        public DataView EvalResultDataView
        {
            get { return _evalResultDataView; }
            set
            {
                _evalResultDataView = value;
                OnPropertyChanged();
            }
        }

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
            Samples = _unitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);

            // for testing:
            _publicationIds.Add(new Guid("D2F66A18-2801-4911-A542-BFE369FE4773"));
            _sampleTable.Columns.Add("Sample");
            _sampleTable.Columns.Add(_unitOfWork.Publications.GetById(_publicationIds.First()).Abbreviation, typeof(bool));

            foreach (var sample in Samples)
            {
                _sampleTable.Rows.Add(new object[] { sample.SampleName, 1 });
            }
            SampleDataView.Sort = "Sample ASC";
        }

        private void SetLabReportIdAndTitle(Guid? id)
        {
            var ReportLabIdent = _unitOfWork.LabReports.GetById((Guid)id).ReportLabIdent;
            LabReportId = id;
            Title = ReportLabIdent;
        }

        private void OnEvalExecute()
        {
            // put here: _evalLabReportParams

            _evalResultTable = new DataTable();
            _evalResultTable.Columns.Add("Sample");

            var r_init = 0;
            var c_init = 1;
            var r = r_init;
            var c = c_init;

            while (c < _sampleTable.Columns.Count)
            {
                _evalResultTable.Columns.Add("ValClass");
                _evalResultTable.Columns.Add("ExceedParam");
                var publicationId = _publicationIds.ElementAt(c - 1);
                while (r < _sampleTable.Rows.Count)
                {
                    if (_sampleTable.Rows[r][c].Equals(true))
                    {
                        var sample = Samples.ElementAt(r);
                        var evalResult = _evalLabReportService.getEvalResult(sample.SampleId, publicationId);
                        // TO DO: use ExpandoObject
                        _evalResultTable.Rows.Add(new object[]
                        { sample.SampleName, evalResult.HighestValClassName, evalResult.ExceedingValueList });
                    }
                r++;
                }
            c++;
            }
            EvalResultDataView = new DataView(_evalResultTable);
            EvalResultDataView.Sort = "Sample ASC";
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
