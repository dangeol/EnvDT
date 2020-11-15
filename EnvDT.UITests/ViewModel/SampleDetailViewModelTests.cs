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
        private List<Publication> _publications;
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
            _publications = new List<Publication>();
            _publications.Add(new Model.Entity.Publication
            {
                PublicationId = new Guid("b2dbef1c-680d-400e-a8a4-7f540b360fa5"),
                Abbreviation = "Publ1"
            });
            _publications.Add(new Model.Entity.Publication
            {
                PublicationId = new Guid("09d6aa16-006f-4e7d-b595-00f747eefad6"),
                Abbreviation = "Publ2"
            });
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.Samples.GetSamplesByLabReportId(_labReportId))
                .Returns(_samples);
            _unitOfWorkMock.Setup(uw => uw.LabReports.GetById(It.IsAny<Guid>()))
                .Returns(_labReport);
            _unitOfWorkMock.Setup(uw => uw.Publications.GetAll())
                .Returns(_publications);
            _evalLabReportServiceMock = new Mock<IEvalLabReportService>();
            _evalLabReportServiceMock.Setup(er => er.LabReportPreCheck(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>()))
                .Returns(true);
            _evalLabReportServiceMock.Setup(er => er.GetEvalResult(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(new Model.Core.HelperClasses.EvalResult());

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

        [Fact]
        public void ShouldBuildCorrectEvalResultDataViewDependingOnSampleDataViewTable()
        {
            _viewModel.Load(_labReportId);
            _viewModel.SampleDataView.Table.Rows[0][1] = true;
            _viewModel.SampleDataView.Table.Rows[0][2] = false;
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = false;

            _viewModel.EvalLabReportCommand.Execute(null);

            Assert.Equal(2, _viewModel.EvalResultDataView.Table.Rows.Count);
            //For each selected publication, two new columns are added; 
            //1 sample name column + 2 new colums = 3.
            Assert.Equal(3, _viewModel.EvalResultDataView.Table.Columns.Count);
            Assert.Equal(_samples[0].SampleName, _viewModel.EvalResultDataView.Table.Rows[0][0]);
            Assert.Equal(_samples[1].SampleName, _viewModel.EvalResultDataView.Table.Rows[1][0]);

            _viewModel.SampleDataView.Table.Rows[0][1] = false;
            _viewModel.SampleDataView.Table.Rows[0][2] = false;
            _viewModel.SampleDataView.Table.Rows[1][1] = true;
            _viewModel.SampleDataView.Table.Rows[1][2] = true;

            _viewModel.EvalLabReportCommand.Execute(null);

            Assert.Equal(1, _viewModel.EvalResultDataView.Table.Rows.Count);
            Assert.Equal(5, _viewModel.EvalResultDataView.Table.Columns.Count);
        }
    }
}
