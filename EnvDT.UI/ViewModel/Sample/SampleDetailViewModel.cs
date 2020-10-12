using EnvDT.Model.Core;
using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
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
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            IsSampleTab = true;
    }

        public ICommand EvalLabReportCommand { get; }

        public ICommand CloseDetailViewCommand { get; }

        public bool IsSampleTab { get; private set; }

        public Guid? LabReportId { get; private set; }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? id)
        {
            System.Diagnostics.Debug.WriteLine("LabReportId: "+id);
            SetLabReportIdAndTitle(id);
        }

        private void SetLabReportIdAndTitle(Guid? id)
        {
            var ReportLabIdent = _unitOfWork.LabReports.GetById((Guid)id).ReportLabIdent;
            LabReportId = id;
            Title = ReportLabIdent;
        }

        private void OnEvalExecute()
        {
            // _evalLabReportService.evalLabReport();
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
