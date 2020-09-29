using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.UI.Event;
using EnvDT.UI.Service;
using EnvDT.UI.ViewModel;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class LabReportViewModelTests
    {
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IOpenLabReportService> _openLabReportServiceMock;
        private Mock<ILabReportDataService> _labReportDataServiceMock;
        private Guid _projectId;
        private Mock<IImportLabReportService> _importLabReportServiceMock;
        private LabReportViewModel _viewModel;
        private LabReportImportedEvent _labReportImportedEvent;
        private string _labReportFilePath = "C:\\Directory\\LabReport.xls";

        public LabReportViewModelTests()
        {
            _labReportImportedEvent = new LabReportImportedEvent();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<LabReportImportedEvent>())
                .Returns(_labReportImportedEvent);
            _openLabReportServiceMock = new Mock<IOpenLabReportService>();
            _openLabReportServiceMock.Setup(ol => ol.OpenLabReport())
                .Returns(_labReportFilePath);
            _labReportDataServiceMock = new Mock<ILabReportDataService>();
            _projectId = new Guid("e26b1ce2-d946-41c7-9edf-ca55b0a47fa0");
            _labReportDataServiceMock.Setup(lr => lr.GetAllLabReportsLookupByProjectId(_projectId))
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = new Guid("09d26650-6d03-4676-a0cb-35ef0052171a"),
                        DisplayMember = "Id111 Lab1"
                    },
                    new LookupItem
                    {
                        LookupItemId = new Guid("190f6042-bdc0-4416-826b-94179458762e"),
                        DisplayMember = "Id222 Lab2"
                    }
                });
            _importLabReportServiceMock = new Mock<IImportLabReportService>();

            _viewModel = new LabReportViewModel(_eventAggregatorMock.Object,
                _labReportDataServiceMock.Object, _openLabReportServiceMock.Object,
                _importLabReportServiceMock.Object);
        }

        [Fact]
        public void ShouldDisableOpenLabReportCommandWhenProjectIdIsNull()
        {
            _viewModel.Load(null);

            Assert.False(_viewModel.OpenLabReportCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableOpenLabReportCommandWhenLabReportIsLoaded()
        {
            _viewModel.Load(_projectId);

            Assert.True(_viewModel.OpenLabReportCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteLabReportCommandWhenLabReportIsLoaded()
        {
            _viewModel.Load(_projectId);

            Assert.False(_viewModel.DeleteLabReportCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldLoadLabReports()
        {
            _viewModel.Load(_projectId);

            Assert.Equal(2, _viewModel.LabReports.Count);

            var labReport = _viewModel.LabReports.SingleOrDefault(
                l => l.LookupItemId == Guid.Parse("09d26650-6d03-4676-a0cb-35ef0052171a"));
            Assert.NotNull(labReport);
            Assert.Equal("Id111 Lab1", labReport.DisplayMember);

            labReport = _viewModel.LabReports.SingleOrDefault(
                l => l.LookupItemId == Guid.Parse("190f6042-bdc0-4416-826b-94179458762e"));
            Assert.NotNull(labReport);
            Assert.Equal("Id222 Lab2", labReport.DisplayMember);
        }

        [Fact]
        public void ShouldLoadLabReportsOnlyOnce()
        {
            _viewModel.Load(_projectId);
            _viewModel.Load(_projectId);

            Assert.Equal(2, _viewModel.LabReports.Count);
        }

        [Fact]
        public void ShouldSetLabReportFilePathAndNameWhenOpenLabReportCommandIsExecuted()
        {
            _viewModel.Load(_projectId);

            _viewModel.OpenLabReportCommand.Execute(null);
            Assert.Equal(_labReportFilePath, _viewModel.LabReportFilePath);
        }
    }
}
