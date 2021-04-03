using EnvDT.Model.Core;
using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class SampleDetailViewModel : DetailViewModelBase, ISampleDetailViewModel
    {
        private IEvalLabReportService _evalLabReportService;
        private ISampleEditDialogViewModel _sampleEditDialogViewModel;
        private IDispatcher _dispatcher;
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
        private string _sampleEditDialogTitle;
        private bool _isColumnEmpty = true;
        private int _footnoteIndex;
        private HashSet<int> _footnoteIndexes;
        private Dictionary<int,string> _footnoteStrings;
        private bool _isEvalResultVisible;
        private bool _isAnimationVisible;
        private string _sampleColHeader;
        private bool _selectSameLrParamMaxValue;
        private bool _selectDiffLrParamMaxValue;
        private bool _evalFootnotes;
        private const string _superscriptDigits =
            "\u2070\u00b9\u00b2\u00b3\u2074\u2075\u2076\u2077\u2078\u2079";

        public SampleDetailViewModel(
            IEventAggregator eventAggregator, IMessageDialogService messageDialogService, 
            IUnitOfWork unitOfWork, IEvalLabReportService evalLabReportService, 
            ISampleEditDialogViewModel sampleEditDialogViewModel, IDispatcher dispatcher)
            : base(eventAggregator, messageDialogService, unitOfWork)
        {
            _evalLabReportService = evalLabReportService;
            _sampleEditDialogViewModel = sampleEditDialogViewModel;
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _sampleTable = new DataTable();
            _selectedPublsTable = new DataTable();
            _selectedPublsTable.Columns.Add("Key");
            _selectedPublsTable.Columns.Add("Value");
            _footnoteStrings = new();
            _footnoteIndexes = new();
            _footnotesTable = new DataTable();
            _footnotesTable.Columns.Add("Key");
            _footnotesTable.Columns.Add("Value");
            _publications = new List<Publication>();
            _selectedPubls = new List<Publication>();
            _sampleEditDialogTitle = Translator["EnvDT.UI.Properties.Strings.SampleDetailVM_DialogTitle_EditSamples"];
            _sampleColHeader = Translator["EnvDT.UI.Properties.Strings.SampleDetailVM_GridHeader_Sample"];
            Samples = new List<Sample>();
            EditSamplesCommand = new DelegateCommand(OnEditSamplesExecute, OnEditSamplesCanExecute);
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            IsSampleTab = true;
            IsEvalResultVisible = false;
            IsAnimationVisible = false;
            SelectSameLrParamMaxValue = true;
            SelectDiffLrParamMaxValue = false;
            EvalFootnotes = true;
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

        public bool IsAnimationVisible
        {
            get { return _isAnimationVisible; }
            set
            {
                _isAnimationVisible = value;
                OnPropertyChanged();
            }
        }

        public bool SelectSameLrParamMaxValue
        {
            get { return _selectSameLrParamMaxValue; }
            set
            {
                _selectSameLrParamMaxValue = value;
                OnPropertyChanged();
            }
        }

        public bool SelectDiffLrParamMaxValue
        {
            get { return _selectDiffLrParamMaxValue; }
            set
            {
                _selectDiffLrParamMaxValue = value;
                OnPropertyChanged();
            }
        }

        public bool EvalFootnotes
        {
            get { return _evalFootnotes; }
            set
            {
                _evalFootnotes = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? labReportId)
        {
            _labReportId = (Guid)labReportId;
            SetLabReportIdAndTitle(labReportId);
            Samples = UnitOfWork.Samples.GetSamplesByLabReportId((Guid)labReportId);
            BuildSampleDataView();
        }

        private void SetLabReportIdAndTitle(Guid? id)
        {
            var ReportLabIdent = UnitOfWork.LabReports.GetById((Guid)id).ReportLabIdent;
            LabReportId = id;
            Title = ReportLabIdent;
        }

        private void BuildSampleDataView()
        {
            _publications = UnitOfWork.Publications.GetAll().OrderBy(p => p.OrderId);
            _sampleTable.Columns.Add(_sampleColHeader);
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
            _sampleEditDialogViewModel.Load(_labReportId);
            MessageDialogService.ShowSampleEditDialog(_sampleEditDialogTitle, _sampleEditDialogViewModel);
        }

        private bool OnEvalCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private async void OnEvalExecute()
        {
            await OnEvalExecuteImpl();
        }

        internal async Task OnEvalExecuteImpl()
        {
            IsEvalResultVisible = false;
            IsAnimationVisible = true;

            try
            {
                await Task.Run(() =>
                {
                    if (LabReportPreCheckSuccess())
                    {
                        Samples = UnitOfWork.Samples.GetSamplesByLabReportId(_labReportId);
                        BuildEvalResultDataView();
                    }
                }).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowOkDialog(
                    Translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_Error"],
                    string.Format(Translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_Error"],
                    ex.Message));
            }
            finally
            {
                if (_evalResultTable != null && _evalResultTable.Rows.Count > 0)
                {
                    IsEvalResultVisible = true;
                }
            }

            IsAnimationVisible = false;
        }

        // TO DO: refactor - find synergies with BuildEvalResultDataView() to increase efficiency
        private bool LabReportPreCheckSuccess()
        {
            bool areSamplesReadyForEval = false;

            _dispatcher.Invoke(() => areSamplesReadyForEval = AreSamplesReadyForEval());

            if (areSamplesReadyForEval)
            {
                var selectedPublIds = _selectedPubls.Select(p => p.PublicationId).ToList();
                return _evalLabReportService.LabReportPreCheck((Guid)LabReportId, selectedPublIds);
            }
            return areSamplesReadyForEval;
        }

        private bool AreSamplesReadyForEval()
        {
            _selectedPubls.Clear();
            _selectedPublsTable.Clear();
            var r_init = 0;
            var c_init = 1;
            var c = c_init;
            var refIndex = 1;

            var needsSampleEditDialog = false;
            _sampleEditDialogViewModel.Load(_labReportId);

            while (c < _sampleTable.Columns.Count)
            {
                var r = r_init;
                var publication = _publications.ElementAt(c - c_init);
                var publicationId = publication.PublicationId;
                var IsCheckBoxInColTrue = false;
                while (r < _sampleTable.Rows.Count)
                {
                    if (_sampleTable.Rows[r][c].Equals(true))
                    {
                        if (!IsCheckBoxInColTrue)
                        {
                            _selectedPubls.Add(publication);
                            var publRef = $"{publication.Publisher} ({publication.Year}): {publication.Title}";
                            DataRow dr = _selectedPublsTable.NewRow();
                            dr["Key"] = $"[{refIndex}]";
                            dr["Value"] = publRef;
                            _selectedPublsTable.Rows.Add(dr);

                            refIndex++;
                        }

                        IsCheckBoxInColTrue = true;

                        var sampleId = Samples.ElementAt(r).SampleId;
                        var sample = UnitOfWork.Samples.GetById(sampleId);
                        var sampleWrapper = _sampleEditDialogViewModel.Samples.Single(s => s.SampleId == sampleId);

                        if (publication.UsesMediumSubTypes
                            && sampleWrapper.MediumSubTypeId != null
                            && Guid.Equals(sampleWrapper.MediumSubTypeId, _sampleEditDialogViewModel.StandardGuid))
                        {
                            needsSampleEditDialog = true;
                            //Trigger validation
                            sampleWrapper.MediumSubTypeId = Guid.Empty;
                        }
                        if (publication.UsesConditions
                            && sampleWrapper.ConditionId != null
                            && Guid.Equals(sampleWrapper.ConditionId, _sampleEditDialogViewModel.StandardGuid))
                        {
                            needsSampleEditDialog = true;
                            //Trigger validation
                            sampleWrapper.ConditionId = Guid.Empty;
                        }                       
                    }
                    r++;
                }
                c++;
            }
            bool isMessageDialogResultAbsentOrOK = true;
            if (needsSampleEditDialog)
            {
                var result = MessageDialogService.ShowSampleEditDialog(_sampleEditDialogTitle, _sampleEditDialogViewModel);
                isMessageDialogResultAbsentOrOK = result == MessageDialogResult.OK;
            }

            return isMessageDialogResultAbsentOrOK;
        }

        private void BuildEvalResultDataView()
        {
            _evalResultTable = new DataTable();
            _evalResultTable.Columns.Add(_sampleColHeader);          
            _footnoteStrings.Clear();
            _footnotesTable.Clear();
            _footnoteIndex = 1;

            var r_init = 0;
            var c_init = 1;
            var c = c_init;
            var c_sampleTable = 1;
            var publListNumber = 1;

            while (c < _sampleTable.Columns.Count)
            {
                _footnoteIndexes.Clear();
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
                PublicationId = publicationId,
                SelectSameLrParamMaxValue = SelectSameLrParamMaxValue,
                SelectDiffLrParamMaxValue = SelectDiffLrParamMaxValue,
                EvalFootnotes = EvalFootnotes
            };
            var evalResult = _evalLabReportService.GetEvalResult(evalArgs);
            _evalResultTable.Rows[r][0] = sample.SampleName;

            var highestValClassName = evalResult.HighestValClassName;
            var exceedingValues = evalResult.ExceedingValues;

            if (evalResult.GeneralFootnoteTexts.Count > 0)
            {
                foreach (string generalFootnoteText in evalResult.GeneralFootnoteTexts)
                {
                    ConstructFootnotes(generalFootnoteText);
                }
                exceedingValues = exceedingValues.Replace("\u2070\u207E", $"{ToSuperscript(_footnoteIndex - 1)}\u207E");
            }
            else
            {
                exceedingValues = exceedingValues.Replace("\u2070\u207E", "");
            }
            if (evalResult.MissingParams.Length > 0)
            {
                var text = Translator["EnvDT.UI.Properties.Strings.SampleDetailVM_Footnote_Missing"];
                var footnoteText = $"{text} {evalResult.MissingParams}";
                ConstructFootnotes(footnoteText);
            }
            if (evalResult.MinValueParams.Length > 0)
            {
                var text = Translator["EnvDT.UI.Properties.Strings.SampleDetailVM_Footnote_MinParams"];
                var footnoteText = $"{text} {evalResult.MinValueParams}";
                ConstructFootnotes(footnoteText);
            }
            if (evalResult.TakingAccountOf.Length > 0)
            {
                var text = Translator["EnvDT.UI.Properties.Strings.SampleDetailVM_Footnote_TakingAccountOf"];
                var footnoteText = $"{text} {evalResult.TakingAccountOf}";
                ConstructFootnotes(footnoteText);
            }
            var cellFootNoteIndexes = "";
            var footnoteIndexesSorted = _footnoteIndexes.OrderBy(item => item);
            foreach (int footnoteIndex in footnoteIndexesSorted)
            {
                cellFootNoteIndexes += $"{ToSuperscript(footnoteIndex)}\u207E";
            };
            highestValClassName += cellFootNoteIndexes;
            _evalResultTable.Rows[r][c_sampleTable] = highestValClassName;
            _evalResultTable.Rows[r][c_sampleTable + 1] = exceedingValues;
        }

        private void ConstructFootnotes(string footnoteText)
        {
            if (!_footnoteStrings.ContainsValue(footnoteText))
            {
                _footnoteStrings.Add(_footnoteIndex, footnoteText);
                DataRow dr = _footnotesTable.NewRow();
                dr["Key"] = $"{ToSuperscript(_footnoteIndex)}\u207E";
                dr["Value"] = footnoteText;
                _footnotesTable.Rows.Add(dr);
                _footnoteIndexes.Add(_footnoteIndex);

                _footnoteIndex++;
            }
            else
            {
                var existingFootnoteIndex = _footnoteStrings.FirstOrDefault(x => x.Value == footnoteText).Key;
                _footnoteIndexes.Add(existingFootnoteIndex);
            }
        }

        private string ToSuperscript(int integer)
        {
            string superscript = new string(integer.ToString()
                                    .Select(x => _superscriptDigits[x - '0'])
                                    .ToArray());
            return superscript;
        }

        private void OnCloseDetailViewExecute()
        {
            EventAggregator.GetEvent<DetailClosedEvent>()
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
