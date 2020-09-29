using EnvDT.Model.IDataService;
using EnvDT.UI.Event;
using EnvDT.UI.Service;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class LabReportViewModel : NavViewModelBase, ILabReportViewModel
    {
        private IEventAggregator _eventAggregator;
        private ILabReportDataService _labReportDataService;
        private IOpenLabReportService _openLabReportService;
        private IImportLabReportService _importLabReportService;
        private Guid? _projectId;

        public LabReportViewModel(IEventAggregator eventAggregator, ILabReportDataService labReportDataService, 
            IOpenLabReportService openLabReportService, IImportLabReportService importLabReportService)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<LabReportImportedEvent>().Subscribe(OnLabReportImported);
            _labReportDataService = labReportDataService;
            _openLabReportService = openLabReportService;
            _importLabReportService = importLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenLabReportExecute, OnOpenLabReportCanExecute);
            DeleteLabReportCommand = new DelegateCommand(OnDeleteLabReportExecute, OnDeleteLabReportCanExecute);

            LabReports = new ObservableCollection<NavItemViewModel>();
        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand DeleteLabReportCommand { get; }
        public ObservableCollection<NavItemViewModel> LabReports { get; }
        public string LabReportFilePath { get; set; }

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

        public override void LoadModels()
        {
            throw new NotImplementedException();
        }

        protected override void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnOpenLabReportExecute()
        {
            LabReportFilePath = _openLabReportService.OpenLabReport();
            System.Diagnostics.Debug.WriteLine(LabReportFilePath);
        }

        private bool OnOpenLabReportCanExecute()
        {
            return _projectId != null;
        }

        private void OnDeleteLabReportExecute()
        {
            throw new NotImplementedException();
        }

        private bool OnDeleteLabReportCanExecute()
        {
            return false;
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
