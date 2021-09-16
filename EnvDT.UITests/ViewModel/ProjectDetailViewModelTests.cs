using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using Xunit;
using System.Collections.ObjectModel;
using EnvDT.Model.IDataService;
using System.Collections.Generic;
using EnvDT.Model.Entity;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectDetailViewModelTests
    {
        private Guid _projectId = new Guid("77ec605f-3909-471f-a866-a2c4759bf5a0");
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<ILookupDataService> _lookupDataServiceMock;
        private Mock<ILabReportViewModel> _labReportViewModelMock;
        private ProjectDetailViewModel _viewModel;
        private Mock<DetailSavedEvent> _projectSavedEventMock;
        private Mock<DetailDeletedEvent> _projectDeletedEventMock;
        private Mock<LabReportImportedEvent> _labReportImportedEventMock;
        private Mock<ISampleDetailViewModel> _sampleDetailViewModelMock;
        private Mock<IProjectViewModel> _projectViewModelMock;
        private Mock<ITab> _tabMock;
        private ObservableCollection<IMainTabViewModel> _tabbedViewModels;
        private Guid _labReportId1 = new Guid("53753aad-6961-45d9-a27f-3a82867519a9");

        public ProjectDetailViewModelTests()
        {
            _projectSavedEventMock = new Mock<DetailSavedEvent>();
            _projectDeletedEventMock = new Mock<DetailDeletedEvent>();
            _labReportImportedEventMock = new Mock<LabReportImportedEvent>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(pr => pr.Projects.GetById(It.IsAny<Guid>()))
                .Returns(new Model.Entity.Project { ProjectId = _projectId, 
                    ProjectNumber = "012345", ProjectName = "name" });
            _eventAggregatorMock = new Mock<IEventAggregator>(); 
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailSavedEvent>())
                .Returns(_projectSavedEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailDeletedEvent>())
                .Returns(_projectDeletedEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<LabReportImportedEvent>())
                .Returns(_labReportImportedEventMock.Object);
            _messageDialogServiceMock = new Mock<IMessageDialogService>();
            _lookupDataServiceMock = new Mock<ILookupDataService>();
            _lookupDataServiceMock.Setup(pr => pr.GetAllCountriesLookup())
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = new Guid("bcc7d1d9-069b-4d7d-83ab-1e53f98a96a6"),
                        DisplayMember = "Country1"
                    },
                    new LookupItem
                    {
                        LookupItemId = new Guid("8f2c0b27-f774-423b-9c2a-40da0ffc8e8f"),
                        DisplayMember = "Country2"
                    }
                });
            _lookupDataServiceMock.Setup(pr => pr.GetAllRegionsLookupByCountryId(It.IsAny<Guid>()))
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = new Guid("20b045c3-9f7c-4dd3-b603-994ddfa61fc2"),
                        DisplayMember = "Region1"
                    },
                    new LookupItem
                    {
                        LookupItemId = new Guid("6840572d-41d1-4db8-b4d9-931ade3cdac4"),
                        DisplayMember = "Region2"
                    }
                });
            _projectViewModelMock = new Mock<IProjectViewModel>();
            _sampleDetailViewModelMock = new Mock<ISampleDetailViewModel>();
            _sampleDetailViewModelMock.SetupGet(vm => vm.LabReportId)
                .Returns(_labReportId1);
            _tabbedViewModels = new ObservableCollection<IMainTabViewModel>();
            _tabMock = new Mock<ITab>();
            _tabbedViewModels.Add(_projectViewModelMock.Object);
            _tabMock.Setup(t => t.TabbedViewModels).Returns(_tabbedViewModels);

            _viewModel = new ProjectDetailViewModel(_unitOfWorkMock.Object, 
                _eventAggregatorMock.Object, _messageDialogServiceMock.Object,
                _lookupDataServiceMock.Object, CreateLabReportViewModel,
                _tabMock.Object);
        }

        private ILabReportViewModel CreateLabReportViewModel()
        {
            var labReportViewModelMock = new Mock<ILabReportViewModel>();
            labReportViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(projectId =>
                {
                    _projectId = projectId.Value;
                    labReportViewModelMock.Setup(vm => vm.LabReports)
                        .Returns(new ObservableCollection<NavItemViewModel>());
                });
            _labReportViewModelMock = labReportViewModelMock;
            return labReportViewModelMock.Object;
        }

        [Fact]
        public void ShouldLoadProject()
        {
            _viewModel.Load(_projectId);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(_projectId, _viewModel.Project.ProjectId);

            _unitOfWorkMock.Verify(uw => uw.Projects.GetById(_projectId), Times.Once);
        }

        [Fact]
        public void ShouldLoadLabReportViewModel()
        {
            _viewModel.Load(_projectId);

            _labReportViewModelMock.Verify(lr => lr.Load(_projectId), Times.Once);
        }
        
        [Fact]
        public void ShoudlRaisePropertyChangedEventForProject()
        {
            var fired = _viewModel.IsPropertyChangedFired(
                () => _viewModel.Load(_projectId),
                nameof(_viewModel.Project));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenProjectIsLoaded()
        {
            _viewModel.Load(_projectId);

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWhenProjectHasValidationErrors()
        {
            _viewModel.Load(_projectId);

            _viewModel.Project.ProjectName = "";

            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableSaveCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            //_viewModel.Project.ProjectNumber = "Changed";
            _viewModel.HasChanges = true;

            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWithoutLoad()
        {
            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

        /* TO DO
        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Project.ProjectNumber = "Changed";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
        {
            var fired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_projectId);
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteCommandWhenAcceptingChanges()
        {
            _viewModel.Load(_projectId);
            var fired = false;
            _viewModel.Project.ProjectNumber = "Changed";
            _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.HasChanges = true;
            Assert.True(fired);
        }   
        
        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteCommandAfterLoad()
        {
            var fired = false;
            _viewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_projectId);
            Assert.True(fired);
        }
        */

        [Fact]
        public void ShouldCallSaveMethodOfProjectRepositoryWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _unitOfWorkMock.Verify(uw => uw.Save(), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);
            Assert.False(_viewModel.HasChanges);
        }

        [Fact]
        public void ShouldPublishProjectSavedEventWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);

            _projectSavedEventMock.Verify(e => e.Publish(It.IsAny<DetailSavedEventArgs>()), Times.Once);
        }

        [Fact]
        public void ShouldCreateNewProjectWhenNullIsPassedToLoadMethod()
        {
            _viewModel.Load(null);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(Guid.Empty, _viewModel.Project.ProjectId);
            Assert.Equal("", _viewModel.Project.ProjectNumber);
            Assert.Equal("", _viewModel.Project.ProjectClient);
            Assert.Equal("", _viewModel.Project.ProjectName);
            Assert.Null(_viewModel.Project.ProjectAddress);

            _unitOfWorkMock.Verify(uw => uw.Projects.GetById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void ShouldEnableDeleteCommandForExistingProject()
        {
            _viewModel.Load(_projectId);

            Assert.True(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandForNewProject()
        {
            _viewModel.Load(null);

            Assert.False(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandWithoudLoad()
        {
            Assert.False(_viewModel.DeleteCommand.CanExecute(null));
        }

        [Theory]
        [InlineData(MessageDialogResult.OK, 1, true)]
        [InlineData(MessageDialogResult.No, 0, false)]
        public void ShouldCallDeleteMethodOfProjectRepositoryWhenDeleteCommandIsExecuted(
            MessageDialogResult result, int expectedDeleteProjectCalls, bool laboratoryReportViewModelIsNull)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteCommand.Execute(null);

            _unitOfWorkMock.Verify(uw => uw.Projects.Delete(_viewModel.Project.Model), 
                Times.Exactly(expectedDeleteProjectCalls));
            Assert.Equal(laboratoryReportViewModelIsNull, _viewModel.LabReportViewModel == null);
            _messageDialogServiceMock.Verify(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(MessageDialogResult.OK, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldPublishProjectDeletedEventWhenDeleteCommandIsExecuted(
            MessageDialogResult result, int expectedPublishCalls)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteCommand.Execute(null);

            _projectDeletedEventMock.Verify(e => e.Publish(It.IsAny<DetailDeletedEventArgs>()), 
                Times.Exactly(expectedPublishCalls));

            _messageDialogServiceMock.Verify(ds => ds.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShouldDisplayCorrectMessageInDeleteDialog()
        {
            _viewModel.Load(_projectId);

            var p = _viewModel.Project;
            p.ProjectClient = "Client";
            p.ProjectName = "ProjectName";

            _viewModel.DeleteCommand.Execute(null);

            _messageDialogServiceMock.Verify(d => d.ShowOkCancelDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShouldNotDeleteSelectedProjectWhenTabsAreOpen()
        {
            _tabbedViewModels.Add(_sampleDetailViewModelMock.Object);
            var _projectIdOpenTab = _labReportId1;
            _viewModel.Load(_projectIdOpenTab);

            _messageDialogServiceMock.Setup(ds => ds.ShowOkDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(MessageDialogResult.OK);

            _viewModel.DeleteCommand.Execute(null);

            _unitOfWorkMock.Verify(uw => uw.Projects.Delete(_viewModel.Project.Model),
                Times.Never);
            Assert.NotNull(_viewModel.LabReportViewModel);
            _messageDialogServiceMock.Verify(ds => ds.ShowOkDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}
