using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Service;
using EnvDT.UI.ViewModel;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class LabReportViewModelTests
    {
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<IOpenLabReportService> _openLabReportServiceMock;
        private Mock<ILabReportDataService> _labReportDataServiceMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ITab> _tabMock;
        private ObservableCollection<IMainTabViewModel> _tabbedViewModels = new ObservableCollection<IMainTabViewModel>();
        private Guid _projectId;
        private Mock<IImportLabReportService> _importLabReportServiceMock;
        private LabReportViewModel _viewModel;
        private LabReportImportedEvent _labReportImportedEvent;

        private string _labReportFilePath = "C:\\Directory\\LabReport.xls";
        private string _labReportFileName = "LabReport.xls";
        private string _reportLabIdent = "ident";
        private Guid _lookupItemId1 = new Guid("09d26650-6d03-4676-a0cb-35ef0052171a");
        private Guid _lookupItemId2 = new Guid("190f6042-bdc0-4416-826b-94179458762e");
        private Guid _labReportId1 = new Guid("53753aad-6961-45d9-a27f-3a82867519a9");
        private Mock<ISampleDetailViewModel> _sampleDetailViewModelMock;

        public LabReportViewModelTests()
        {
            _labReportImportedEvent = new LabReportImportedEvent();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<LabReportImportedEvent>())
                .Returns(_labReportImportedEvent);
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _messageDialogServiceMock.Setup(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(MessageDialogResult.OK);
            _openLabReportServiceMock = new Mock<IOpenLabReportService>();
            _openLabReportServiceMock.Setup(ol => ol.OpenLabReport())
                .Returns(_labReportFilePath);
            _labReportDataServiceMock = new Mock<ILabReportDataService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uw => uw.LabReports.GetById(_lookupItemId1))
                .Returns(new Model.Entity.LabReport
                {
                    LabReportId = _lookupItemId1,
                    ReportLabIdent = _reportLabIdent
                });
            _sampleDetailViewModelMock = new Mock<ISampleDetailViewModel>();
            _sampleDetailViewModelMock.SetupGet(vm => vm.LabReportId)
                .Returns(_labReportId1);
            _tabbedViewModels.Add(_sampleDetailViewModelMock.Object);
            _tabMock = new Mock<ITab>();
            _tabMock.Setup(t => t.GetTabbedViewModelByEventArgs(It.IsAny<IDetailEventArgs>()))
                .Returns(_tabbedViewModels.First());
            _projectId = new Guid("e26b1ce2-d946-41c7-9edf-ca55b0a47fa0");
            _labReportDataServiceMock.Setup(lr => lr.GetAllLabReportsLookupByProjectId(_projectId))
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = _lookupItemId1,
                        DisplayMember = "Id111 Lab1"
                    },
                    new LookupItem
                    {
                        LookupItemId = _lookupItemId2,
                        DisplayMember = "Id222 Lab2"
                    }
                });
            _importLabReportServiceMock = new Mock<IImportLabReportService>();

            _viewModel = new LabReportViewModel(_eventAggregatorMock.Object,
                _messageDialogServiceMock.Object, _labReportDataServiceMock.Object,
                _unitOfWorkMock.Object, _tabMock.Object, _openLabReportServiceMock.Object, 
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

        [Fact]
        public void ShouldSetTheCorrectLabReportFileNameWhenOpenLabReportCommandIsExecuted()
        {
            _viewModel.Load(_projectId);

            _viewModel.OpenLabReportCommand.Execute(null);

            Assert.Equal(_labReportFileName, _viewModel.LabReportFileName);
        }

        [Fact]
        public void ShouldDisableImportLabReportCommandAndDeleteLabReportCommandWhenNoFileIsOpened()
        {
            Assert.False(_viewModel.ImportLabReportCommand.CanExecute(null));
            Assert.False(_viewModel.DeleteLabReportCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableImportLabReportCommandWhenLabReportIsOpened()
        {
            _viewModel.Load(_projectId);

            _viewModel.OpenLabReportCommand.Execute(null);

            Assert.True(_viewModel.ImportLabReportCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldAddLabReportItemWhenNewLabReportHasBeenImported()
        {
            _viewModel.Load(_projectId);

            var newLabReportId = new Guid("7ddc7822-7447-4210-a5d4-cfb88e5a7655");
            var reportLabIdent = "New1234";
            var laboratoryName = "NewLab";

            _labReportImportedEvent.Publish(
                new LabReportImportedEventArgs
                {
                    Id = newLabReportId,
                    DisplayMember = $"{reportLabIdent} {laboratoryName}"
                });

            Assert.Equal(3, _viewModel.LabReports.Count);

            var addLabReportItem = _viewModel.LabReports.SingleOrDefault(lr => lr.LookupItemId == newLabReportId);
            Assert.NotNull(addLabReportItem);
            Assert.Equal("New1234 NewLab", addLabReportItem.DisplayMember);
        }

        [Fact]
        public void ShouldNotAddLabReportItemWhichAlreadyExists()
        {
            _viewModel.Load(_projectId);

            _viewModel.OpenLabReportCommand.Execute(null);
            _importLabReportServiceMock.Setup(lr => lr.IsLabReportAlreadyPresent(_reportLabIdent))
                .Returns(true);

            Assert.Equal(2, _viewModel.LabReports.Count);
        }

        [Fact]
        public void ShouldEnableDeleteLabReportCommandWhenLabReportIsSelected()
        {
            _viewModel.Load(_projectId);

            _viewModel.SelectedLabReport = new NavItemViewModel(new Guid(), "", "", _eventAggregatorMock.Object);

            Assert.True(_viewModel.DeleteLabReportCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldNotDeleteSelectedLabReportWhenRelatedTabIsOpen()
        {
            _viewModel.Load(_projectId);

            _viewModel.SelectedLabReport = new NavItemViewModel(_labReportId1, "", "", _eventAggregatorMock.Object);
            _viewModel.DeleteLabReportCommand.Execute(null);

            Assert.Equal(2, _viewModel.LabReports.Count);

            _messageDialogServiceMock.Verify(ds => ds.ShowOkDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 2)]
        public void ShouldDeleteSelectedLabReportWhenDeleteLabReportCommandIsExecuted(
            MessageDialogResult result, int expectedLabReportsCount)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.SelectedLabReport = new NavItemViewModel(_lookupItemId1, "", "", _eventAggregatorMock.Object);
            _viewModel.DeleteLabReportCommand.Execute(null);

            Assert.Equal(expectedLabReportsCount, _viewModel.LabReports.Count);

            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShouldPublishOpenDetailViewEvent()
        {
            var eventMock = new Mock<OpenDetailViewEvent>();

            _eventAggregatorMock
                .Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(eventMock.Object);
            _viewModel.SelectedLabReport = new NavItemViewModel(
                Guid.NewGuid(), "", "SampleDetailViewModel", _eventAggregatorMock.Object);

            _viewModel.OpenDetailViewCommand.Execute(null);

            eventMock.Verify(e => e.Publish(It.IsAny<OpenDetailViewEventArgs>()), Times.Once);
        }
    }
}
