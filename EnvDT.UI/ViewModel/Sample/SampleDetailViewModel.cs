﻿using EnvDT.Model.Core;
using EnvDT.Model.Core.HelperClasses;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class SampleDetailViewModel : DetailViewModelBase, ISampleDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IUnitOfWork _unitOfWork;
        private IEvalLabReportService _evalLabReportService;
        private ISampleEditDialogViewModel _sampleEditDialogViewModel;
        private Guid _labReportId;
        private IEnumerable<Publication> _publications;
        private List<Publication> _selectedPubls;
        private DataTable _sampleTable;
        private DataTable _evalResultTable;
        private DataTable _footnotesTable;
        private DataTable _selectedPublsTable;
        private DataView _sampleDataView;
        private DataView _evalResultDataView;
        private DataView _footnotesDataView;
        private DataView _selectedPublsDataView;
        private string _title = "Project";
        private string _sampleEditDialogTitle = "Edit samples";
        private bool _isColumnEmpty = true;
        private int _footnoteIndex;
        private bool _isEvalResultVisible;

        public SampleDetailViewModel(
            IEventAggregator eventAggregator, IMessageDialogService messageDialogService, 
            IUnitOfWork unitOfWork, IEvalLabReportService evalLabReportService, 
            ISampleEditDialogViewModel sampleEditDialogViewModel)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _unitOfWork = unitOfWork;
            _evalLabReportService = evalLabReportService;
            _sampleEditDialogViewModel = sampleEditDialogViewModel;
            _sampleTable = new DataTable();
            _selectedPublsTable = new DataTable();
            _selectedPublsTable.Columns.Add("Key");
            _selectedPublsTable.Columns.Add("Value");
            _footnotesTable = new DataTable();
            _footnotesTable.Columns.Add("Key");
            _footnotesTable.Columns.Add("Value");
            _publications = new List<Publication>();
            _selectedPubls = new List<Publication>();
            Samples = new List<Sample>();
            EditSamplesCommand = new DelegateCommand(OnEditSamplesExecute, OnEditSamplesCanExecute);
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            IsSampleTab = true;
            IsEvalResultVisible = false;
        }

        public ICommand EditSamplesCommand { get; }
        public ICommand EvalLabReportCommand { get; }
        public ICommand CloseDetailViewCommand { get; }

        public bool IsSampleTab { get; private set; }
        public Guid? LabReportId { get; set; }
        public IEnumerable<Sample> Samples { get; private set; }

        // Title of current tab
        public string Title { get; private set; }

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

        public DataView FootnotesDataView
        {
            get { return _footnotesDataView; }
            private set
            {
                _footnotesDataView = value;
                OnPropertyChanged();
            }
        }

        public DataView SelectedPublsDataView
        {
            get { return _selectedPublsDataView; }
            private set
            {
                _selectedPublsDataView = value;
                OnPropertyChanged();
            }
        }

        public bool IsEvalResultVisible
        {
            get { return _isEvalResultVisible; }
            set
            {
                _isEvalResultVisible = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? labReportId)
        {
            _labReportId = (Guid)labReportId;
            SetLabReportIdAndTitle(labReportId);
            Samples = _unitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);
            _sampleEditDialogViewModel.Load(_labReportId);
            BuildSampleDataView();
        }

        private void SetLabReportIdAndTitle(Guid? id)
        {
            var ReportLabIdent = _unitOfWork.LabReports.GetById((Guid)id).ReportLabIdent;
            LabReportId = id;
            Title = ReportLabIdent;
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
        }

        private bool OnEditSamplesCanExecute()
        {
            return true;
        }

        private void OnEditSamplesExecute()
        {
            _messageDialogService.ShowSampleEditDialog(_sampleEditDialogTitle, _sampleEditDialogViewModel);
        }

        private bool OnEvalCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnEvalExecute()
        {
            if (LabReportPreCheckSuccess())
            {
                BuildEvalResultDataView();
            }
            if (EvalResultDataView != null)
            {
                IsEvalResultVisible = true;
            }
        }

        // TO DO: refactor - find synergies with BuildEvalResultDataView() to increase efficiency
        private bool LabReportPreCheckSuccess()
        {
            _selectedPubls.Clear();
            _selectedPublsTable.Clear();
            var r_init = 0;
            var c_init = 1;
            var c = c_init;
            var refIndex = 1;

            var isUsingMedSubTypes = false;
            var isUsingConditions = false;

            while (c < _sampleTable.Columns.Count)
            {
                var r = r_init;
                var publication = _publications.ElementAt(c - c_init);
                var publicationId = publication.PublicationId;
                var IsCheckBoxInColTrue = false;
                while (r < _sampleTable.Rows.Count && !IsCheckBoxInColTrue)
                {
                    if (_sampleTable.Rows[r][c].Equals(true))
                    {
                        IsCheckBoxInColTrue = true;
                        _selectedPubls.Add(publication);
                        var publRef = $"{publication.Publisher} ({publication.Year}): {publication.Title}";
                        DataRow dr = _selectedPublsTable.NewRow();
                        dr["Key"] = $"[{refIndex}]";
                        dr["Value"] = publRef;
                        _selectedPublsTable.Rows.Add(dr);

                        refIndex++;

                        if (publication.UsesMediumSubTypes)
                        {
                            isUsingMedSubTypes = true;
                        }
                        if (publication.UsesConditions)
                        {
                            isUsingConditions = true;
                        }
                    }
                    r++;
                }
                c++;
            }
            if (isUsingMedSubTypes || isUsingConditions)
            {
                var result = _messageDialogService.ShowSampleEditDialog(_sampleEditDialogTitle, _sampleEditDialogViewModel);
                return result == MessageDialogResult.OK;
            }
            var selectedPublIds = _selectedPubls.Select(p => p.PublicationId).ToList();
            return _evalLabReportService.LabReportPreCheck((Guid)LabReportId, selectedPublIds);
        }

        private void BuildEvalResultDataView()
        {
            _evalResultTable = new DataTable();
            _evalResultTable.Columns.Add("Sample");
            _footnotesTable.Clear();
            _footnoteIndex = 1;

            var r_init = 0;
            var c_init = 1;
            var c = c_init;
            var c_sampleTable = 1;
            var publListNumber = 1;

            while (c < _sampleTable.Columns.Count)
            {
                var r = r_init;
                var publication = _publications.ElementAt(c - c_init);
                var publicationId = publication.PublicationId;
                _evalResultTable.Columns.Add($"ValClass{c}");
                _evalResultTable.Columns.Add($"ExceedParam{c}");
                _isColumnEmpty = true;
                while (r < _sampleTable.Rows.Count)
                {
                    if (c == c_init)
                    {
                        DataRow dr = _evalResultTable.NewRow();
                        _evalResultTable.Rows.Add(dr);
                    }
                    if (_sampleTable.Rows[r][c].Equals(true))
                    {
                        FillTwoResultTableCells(c_sampleTable, r, publicationId);
                    }
                    r++;
                }
                if (_isColumnEmpty)
                {
                    _evalResultTable.Columns.Remove($"ValClass{c}");
                    _evalResultTable.Columns.Remove($"ExceedParam{c}");
                }
                else
                {
                    c_sampleTable += 2;
                    var _colCount = _evalResultTable.Columns.Count;
                    _evalResultTable.Columns[_colCount - 2].ColumnName = $"V{publListNumber}";
                    _evalResultTable.Columns[_colCount - 1].ColumnName = $"E{publListNumber}";
                    publListNumber++;
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
            FootnotesDataView = new DataView(_footnotesTable);
            SelectedPublsDataView = new DataView(_selectedPublsTable);
        }

        private void FillTwoResultTableCells(int c_sampleTable, int r, Guid publicationId)
        {
            _isColumnEmpty = false;
            var sample = Samples.ElementAt(r);
            var evalArgs = new EvalArgs
            {
                LabReportId = (Guid)LabReportId,
                Sample = sample,
                PublicationId = publicationId
            };
            var evalResult = _evalLabReportService.GetEvalResult(evalArgs);
            _evalResultTable.Rows[r][0] = sample.SampleName;
            var highestValClassName = evalResult.HighestValClassName;
            if (evalResult.MissingParams.Length == 0)
            {
                _evalResultTable.Rows[r][c_sampleTable] = highestValClassName;
            }
            else
            {
                _evalResultTable.Rows[r][c_sampleTable] = $"{highestValClassName} {_footnoteIndex})";
                var missingParamFootNote = $"Missing: {evalResult.MissingParams}";
                DataRow dr = _footnotesTable.NewRow();
                dr["Key"] = $"{_footnoteIndex})";
                dr["Value"] = missingParamFootNote;
                _footnotesTable.Rows.Add(dr);

                _footnoteIndex++;
            }
            _evalResultTable.Rows[r][c_sampleTable + 1] = evalResult.ExceedingValues;
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
