using EnvDT.Model;
using EnvDT.UI.Data.Repository;
using EnvDT.UI.Data.Service;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IOpenLabReportService _openLabReportService;
        private IEvalLabReportService _evalLabReportService;
        private IProjectRepository _projectRepository;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            Projects = new ObservableCollection<ProjectItemViewModel>();
        }

        public NavigationViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator,
            IOpenLabReportService openLabReportService, IEvalLabReportService evalLabReportService)
        {

            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _openLabReportService = openLabReportService;
            _evalLabReportService = evalLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenExecute, OnOpenCanExecute);
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);

            Projects = new ObservableCollection<ProjectItemViewModel>();
        }

        private void OnOpenExecute()
        {
            _openLabReportService.OpenLabReport();
        }

        private bool OnOpenCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnEvalExecute()
        {
            _evalLabReportService.evalLabReport();
        }

        private bool OnEvalCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        public void LoadProjects()
        {
            Projects.Clear();
            foreach (var project in _projectRepository.GetAllProjects())
            {
                Projects.Add(new ProjectItemViewModel(
                    project.LookupItemId, project.DisplayMember, _eventAggregator));
            }
        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand EvalLabReportCommand { get; }

        public ObservableCollection<ProjectItemViewModel> Projects { get; private set; }
        public Project SelectedProject { get; set; }
    }
}
