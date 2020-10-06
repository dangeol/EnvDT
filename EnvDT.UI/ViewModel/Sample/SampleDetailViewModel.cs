using EnvDT.Model.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class SampleDetailViewModel : DetailViewModelBase, ISampleDetailViewModel
    {
        private IEvalLabReportService _evalLabReportService;

        public SampleDetailViewModel(IEventAggregator eventAggregator, IEvalLabReportService evalLabReportService)
            : base(eventAggregator)
        {
            _evalLabReportService = evalLabReportService;

            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
        }

        public ICommand EvalLabReportCommand { get; }

        public override void Load(Guid? id)
        {
            throw new NotImplementedException();
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
