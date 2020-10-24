using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Service;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class LabReportViewModel : NavItemViewModel, ILabReportViewModel
    {
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private ILabReportDataService _labReportDataService;
        private IUnitOfWork _unitOfWork;
        private ITab _tab;
        private IOpenLabReportService _openLabReportService;
        private IImportLabReportService _importLabReportService;
        private Guid? _projectId;
        private string _labReportFileName;
        private NavItemViewModel _selectedLabReport;
        private const string _sampleDetailViewModelName = "SampleDetailViewModel";

        public LabReportViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            ILabReportDataService labReportDataService, IUnitOfWork unitOfWork, ITab tab,
            IOpenLabReportService openLabReportService, IImportLabReportService importLabReportService)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<LabReportImportedEvent>().Subscribe(OnLabReportImported);
            _messageDialogService = messageDialogService;
            _labReportDataService = labReportDataService;
            _unitOfWork = unitOfWork;
            _tab = tab;
            _openLabReportService = openLabReportService;
            _importLabReportService = importLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenLabReportExecute, OnOpenLabReportCanExecute);
            ImportLabReportCommand = new DelegateCommand(OnImportLabReportExecute, OnImportLabReportCanExecute);
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute, OnOpenDetailViewCanExecute);
            DeleteLabReportCommand = new DelegateCommand(OnDeleteLabReportExecute, OnDeleteLabReportCanExecute);

            LabReports = new ObservableCollection<NavItemViewModel>();
        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand ImportLabReportCommand { get; }
        public ICommand DeleteLabReportCommand { get; }
        public ObservableCollection<NavItemViewModel> LabReports { get; }
        public string LabReportFilePath { get; set; }

        public string LabReportFileName
        {
            get { return _labReportFileName; }
            set 
            { 
                _labReportFileName = value;
                OnPropertyChanged();
            }
        }

        public NavItemViewModel SelectedLabReport
        {
            get { return _selectedLabReport; }
            set 
            { 
                _selectedLabReport = value;
                if (_selectedLabReport != null)
                {
                    LookupItemId = _selectedLabReport.LookupItemId;
                }
                DetailViewModelName = _sampleDetailViewModelName;
                OnPropertyChanged();
                ((DelegateCommand)DeleteLabReportCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)OpenDetailViewCommand).RaiseCanExecuteChanged();
            }
        }

        public void Load(Guid? projectId)
        {
            _projectId = projectId;

            ((DelegateCommand)OpenLabReportCommand).RaiseCanExecuteChanged();
            LabReports.Clear();

            foreach (var labReport in _labReportDataService.GetAllLabReportsLookupByProjectId(projectId))
            {
                LabReports.Add(new NavItemViewModel(
                    labReport.LookupItemId, labReport.DisplayMember, "",
                    _eventAggregator));
            }
        }

        private void OnOpenLabReportExecute()
        {
            LabReportFilePath = _openLabReportService.OpenLabReport();

            Regex lastSlashRgx = new Regex(@"([\/\\])(?!.*\1)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match lastSlashMatch = lastSlashRgx.Match(LabReportFilePath);
            if (lastSlashMatch.Success)
            {
                int matchIndex = lastSlashMatch.Index;
                LabReportFileName = LabReportFilePath.Substring(matchIndex + 1);
                ((DelegateCommand)ImportLabReportCommand).RaiseCanExecuteChanged();
            }
        }

        private bool OnOpenLabReportCanExecute()
        {
            return _projectId != null;
        }

        private void OnImportLabReportExecute()
        {
            _importLabReportService.ImportLabReport(LabReportFilePath, _projectId);
        }

        private bool OnImportLabReportCanExecute()
        {
            return LabReportFilePath != null;
        }

        protected override bool OnOpenDetailViewCanExecute()
        {
            return SelectedLabReport != null;
        }

        private void OnDeleteLabReportExecute()
        {
            var detailEventArgs = new DetailClosedEventArgs
            {
                Id = SelectedLabReport.LookupItemId,
                ViewModelName = _sampleDetailViewModelName
            };

            var openTab = _tab.GetTabbedViewModelByEventArgs(detailEventArgs);
            if (openTab != null)
            {
                _messageDialogService.ShowOkDialog("Delete LabReport",
                    $"Please close first related open Tab '{SelectedLabReport.DisplayMember}'.");
                return;
            }

            var result = _messageDialogService.ShowOkCancelDialog("Delete LabReport",
                $"Do you really want to delete the LabReport '{SelectedLabReport.DisplayMember}' " +
                $"and all related samples and values?");
            if (result == MessageDialogResult.OK)
            {
                var labReport = _unitOfWork.LabReports.GetById(SelectedLabReport.LookupItemId);
                var labReportItem = LabReports.SingleOrDefault(lr => lr.LookupItemId == labReport.LabReportId);
                if (labReportItem != null)
                {
                    LabReports.Remove(labReportItem);
                }
                _unitOfWork.LabReports.Delete(labReport);
                _unitOfWork.Save();
            }
        }

        private bool OnDeleteLabReportCanExecute()
        {
            return SelectedLabReport != null;
        }

        private void OnLabReportImported(LabReportImportedEventArgs args)
        {
            var displayMember = args.DisplayMember;

            var labReportItem = LabReports.SingleOrDefault(l => l.LookupItemId == args.Id);
            if (labReportItem != null)
            {
                labReportItem.DisplayMember = displayMember;
            }
            else
            {
                labReportItem = new NavItemViewModel(args.Id, displayMember, "",
                    _eventAggregator);
                LabReports.Add(labReportItem);
            }
        }
    }
}
