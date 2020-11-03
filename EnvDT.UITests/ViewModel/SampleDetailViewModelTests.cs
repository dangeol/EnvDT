using EnvDT.Model.IRepository;
using EnvDT.Model.Core;
using Moq;
using Prism.Events;
using System;
using Xunit;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Event;
using EnvDT.Model.Entity;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.UITests.ViewModel
{
    public class SampleDetailViewModelTests
    {
        private Guid _labReportId = new Guid("35b396f3-d755-49a6-a748-54b55e46a6c6");
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEvalLabReportService> _evalLabReportServiceMock;
        private SampleDetailViewModel _viewModel;
        private Mock<DetailClosedEvent> _detailClosedEventMock;
        private List<Sample> _samples;
        private LabReport _labReport;
        private Publication _publication;
        private string _reportLabIdent = "ident";

        public SampleDetailViewModelTests()
        {
            _detailClosedEventMock = new Mock<DetailClosedEvent>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailClosedEvent>())
                .Returns(_detailClosedEventMock.Object);
            _samples = new List<Sample>();
            _samples.Add(new Model.Entity.Sample
            {
                SampleId = new Guid("1f343ed6-e410-4f4b-9432-073acada899b"),
                SampleName = "Sample1"
            });
            _samples.Add(new Model.Entity.Sample
            {
                SampleId = new Guid("f515fda2-e370-4a22-b20c-4ffdc5c12503"),
                SampleName = "Sample2"
            });
            _labReport = new LabReport();
            _labReport.ReportLabIdent = _reportLabIdent;
            _publication = new Publication();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.Samples.GetSamplesByLabReportId(_labReportId))
                .Returns(_samples);
            _unitOfWorkMock.Setup(uw => uw.LabReports.GetById(It.IsAny<Guid>()))
                .Returns(_labReport);
            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);
            _evalLabReportServiceMock = new Mock<IEvalLabReportService>();

            _viewModel = new SampleDetailViewModel(_eventAggregatorMock.Object, 
                _unitOfWorkMock.Object, _evalLabReportServiceMock.Object);
        }

        [Fact]
        public void ShouldLoadSamplesAndSetTabTitle()
        {
            _viewModel.Load(_labReportId);

            Assert.Equal(2, _viewModel.Samples.Count());

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
