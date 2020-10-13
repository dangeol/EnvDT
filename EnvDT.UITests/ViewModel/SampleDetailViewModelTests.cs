using EnvDT.Model.IRepository;
using EnvDT.Model.Core;
using Moq;
using Prism.Events;
using System;
using Xunit;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Event;
using EnvDT.Model.Entity;

namespace EnvDT.UITests.ViewModel
{
    public class SampleDetailViewModelTests
    {
        private Guid _labReportId = new Guid("2e3ce7d6-b1bf-43a1-b395-0fce9b08a71d");
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEvalLabReportService> _evalLabReportServiceMock;
        private SampleDetailViewModel _viewModel;
        private Mock<DetailClosedEvent> _detailClosedEventMock;
        private LabReport _labReport;
        private string _reportLabIdent = "ident";

        public SampleDetailViewModelTests()
        {
            _detailClosedEventMock = new Mock<DetailClosedEvent>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailClosedEvent>())
                .Returns(_detailClosedEventMock.Object);
            _labReport = new LabReport();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _evalLabReportServiceMock = new Mock<IEvalLabReportService>();
            _labReport.ReportLabIdent = _reportLabIdent;
            _unitOfWorkMock.Setup(uw => uw.LabReports.GetById(It.IsAny<Guid>()))
                .Returns(_labReport);

            _viewModel = new SampleDetailViewModel(_eventAggregatorMock.Object, 
                _unitOfWorkMock.Object, _evalLabReportServiceMock.Object);
        }

        [Fact]
        public void ShouldLoadSamplesAndSetTabTitle()
        {
            _viewModel.Load(_labReportId);

            Assert.NotNull(_viewModel.Title);
            Assert.Equal(_viewModel.Title, _reportLabIdent);
        }

        [Fact]
        public void ShouldPublishDetailClosedEventWhenCloseDetailViewCommandIsExecuted()
        {
            _viewModel.Load(_labReportId);

            _viewModel.CloseDetailViewCommand.Execute(null);

            _detailClosedEventMock.Verify(e => e.Publish(It.IsAny<DetailClosedEventArgs>()), Times.Once);
        }
    }
}
