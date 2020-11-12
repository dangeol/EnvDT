﻿using EnvDT.Model.Core;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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
        private IEnumerable<Publication> _publications;
        private List<Guid> _selectedPublIds;
        private DataView _sampleDataView;
        private DataView _evalResultDataView;
        private string _title = "Project";
        private bool isColumnEmpty = true;

        public SampleDetailViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork,
            IEvalLabReportService evalLabReportService)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _evalLabReportService = evalLabReportService;
            _sampleTable = new DataTable();
            _publications = new List<Publication>();
            _selectedPublIds = new List<Guid>();
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
            private set
            {
                _evalResultDataView = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            private set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? labReportId)
        {
            SetLabReportIdAndTitle(labReportId);
            Samples = _unitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);
            BuildSampleDataView();
        }

        private void BuildSampleDataView()
        {
            _publications = _unitOfWork.Publications.GetAll().OrderBy(p => p.OrderId);
            _sampleTable.Columns.Add("Sample");
            IDictionary<string, object> sampleTableRow = new ExpandoObject();
            var sampleNameKey = "SampleName";
            sampleTableRow[sampleNameKey] = "";
            foreach (var publication in _publications)
            {
                _sampleTable.Columns.Add(publication.Abbreviation, typeof(bool));
                var publColName = $"publ_{publication.OrderId}";
                sampleTableRow[publColName] = 0;
            }

            foreach (var sample in Samples)
            {
                sampleTableRow[sampleNameKey] = sample.SampleName;
                _sampleTable.Rows.Add(sampleTableRow.Values.ToArray());
            }
            SampleDataView = new DataView(_sampleTable);
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
            LabReportPreCheck();
            BuildEvalResultDataView();
        }

        // TO DO: refactor - find synergies with BuildEvalResultDataView() to increase efficiency
        private void LabReportPreCheck()
        {
            _selectedPublIds.Clear();
            var r_init = 0;
            var c_init = 1;
            var c = c_init;

            while (c < _sampleTable.Columns.Count)
            {
                var r = r_init;
                var publication = _publications.ElementAt(c - 1);
                var publicationId = publication.PublicationId;
                var IsCheckBoxInColTrue = false;
                while (r < _sampleTable.Rows.Count && !IsCheckBoxInColTrue)
                {
                    if (_sampleTable.Rows[r][c].Equals(true))
                    {
                        IsCheckBoxInColTrue = true;
                        _selectedPublIds.Add(publicationId);
                    }
                    r++;
                }
                c++;
            }
            _evalLabReportService.LabReportPreCheck((Guid)LabReportId, _selectedPublIds);
        }

        private void BuildEvalResultDataView()
        {
            _evalResultTable = new DataTable();
            _evalResultTable.Columns.Add("Sample");

            var r_init = 0;
            var c_init = 1;
            var c = c_init;

            while (c < _sampleTable.Columns.Count)
            {
                var r = r_init;
                var publication = _publications.ElementAt(c - 1);
                var publicationId = publication.PublicationId;
                _evalResultTable.Columns.Add($"ValClass{c}");
                _evalResultTable.Columns.Add($"ExceedParam{c}");
                isColumnEmpty = true;
                while (r < _sampleTable.Rows.Count)
                {
                    if (c == c_init)
                    {
                        DataRow dr = _evalResultTable.NewRow();
                        _evalResultTable.Rows.Add(dr);
                    }
                    if (_sampleTable.Rows[r][c].Equals(true))
                    {
                        isColumnEmpty = false;
                        var sample = Samples.ElementAt(r);
                        var evalResult = _evalLabReportService.GetEvalResult((Guid)LabReportId, sample.SampleId, publicationId);
                        _evalResultTable.Rows[r][0] = sample.SampleName;
                        _evalResultTable.Rows[r][c] = evalResult.HighestValClassName;
                        _evalResultTable.Rows[r][c+1] = evalResult.ExceedingValueList;
                    }
                    r++;
                }
                if (isColumnEmpty)
                {
                    _evalResultTable.Columns.Remove($"ValClass{c}");
                    _evalResultTable.Columns.Remove($"ExceedParam{c}");
                }
                c++;
            }
            //Remove empty rows:
            for (int row = _evalResultTable.Rows.Count - 1; row >= 0; row--)
            {
                if (_evalResultTable.Rows[row][0] == System.DBNull.Value)
                {
                    _evalResultTable.Rows.RemoveAt(row);
                }
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
