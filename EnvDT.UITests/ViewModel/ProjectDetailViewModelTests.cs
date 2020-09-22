using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectDetailViewModelTests
    {
        private Guid _projectId = new Guid("77ec605f-3909-471f-a866-a2c4759bf5a0");
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private ProjectDetailViewModel _viewModel;
        private Mock<DetailSavedEvent> _projectSavedEventMock;
        private Mock<DetailDeletedEvent> _projectDeletedEventMock;

        public ProjectDetailViewModelTests()
        {
            _projectSavedEventMock = new Mock<DetailSavedEvent>();
            _projectDeletedEventMock = new Mock<DetailDeletedEvent>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(pr => pr.GetProjectById(_projectId))
                .Returns(new Model.Entity.Project { ProjectId = _projectId, 
                    ProjectNumber = "012345", ProjectName = "name" });
            _eventAggregatorMock = new Mock<IEventAggregator>(); 
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailSavedEvent>())
                .Returns(_projectSavedEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailDeletedEvent>())
                .Returns(_projectDeletedEventMock.Object);

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _viewModel = new ProjectDetailViewModel(_projectRepositoryMock.Object, 
                _eventAggregatorMock.Object, _messageDialogServiceMock.Object);
        }

        [Fact]
        public void ShouldLoadProject()
        {
            _viewModel.Load(_projectId);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(_projectId, _viewModel.Project.ProjectId);

            _projectRepositoryMock.Verify(pr => pr.GetProjectById(_projectId), Times.Once);
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
        public void ShouldDisableSaveProjectCommandWhenProjectIsLoaded()
        {
            _viewModel.Load(_projectId);

            Assert.False(_viewModel.SaveProjectCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveProjectCommandWhenProjectHasValidationErrors()
        {
            _viewModel.Load(_projectId);

            _viewModel.Project.ProjectName = "";

            Assert.False(_viewModel.SaveProjectCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldEnableSaveProjectCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            //_viewModel.Project.ProjectNumber = "Changed";
            _viewModel.HasChanges = true;

            Assert.True(_viewModel.SaveProjectCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveProjectCommandWithoutLoad()
        {
            Assert.False(_viewModel.SaveProjectCommand.CanExecute(null));
        }

        /* TO DO
        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveProjectCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            var fired = false;
            _viewModel.SaveProjectCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Project.ProjectNumber = "Changed";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveProjectCommandAfterLoad()
        {
            var fired = false;
            _viewModel.SaveProjectCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_projectId);
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteProjectCommandWhenAcceptingChanges()
        {
            _viewModel.Load(_projectId);
            var fired = false;
            _viewModel.Project.ProjectNumber = "Changed";
            _viewModel.DeleteProjectCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.HasChanges = true;
            Assert.True(fired);
        }   
        
        [Fact]
        public void ShouldRaiseCanExecuteChangedForDeleteProjectCommandAfterLoad()
        {
            var fired = false;
            _viewModel.DeleteProjectCommand.CanExecuteChanged += (s, e) => fired = true;
            _viewModel.Load(_projectId);
            Assert.True(fired);
        }
        */

        [Fact]
        public void ShouldCallSaveMethodOfProjectRepositoryWhenSaveProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveProjectCommand.Execute(null);
            _projectRepositoryMock.Verify(pr => pr.Save(), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveProjectCommand.Execute(null);
            Assert.False(_viewModel.HasChanges);
        }

        // TO DO: Find reason why this test fails. Same case as ShouldPublishOpenDetailViewEvent test.
        [Fact]
        public void ShouldPublishProjectSavedEventWhenSaveProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";
            var detailSavedEventArgs = new DetailSavedEventArgs
            {
                Id = _viewModel.Project.Model.ProjectId,
                DisplayMember = $"{_viewModel.Project.Model.ProjectNumber} {_viewModel.Project.Model.ProjectName}",
                ViewModelName = nameof(ProjectDetailViewModel)
            };

            _viewModel.SaveProjectCommand.Execute(null);

            _projectSavedEventMock.Verify(e => e.Publish(detailSavedEventArgs), Times.Once);
        }

        [Fact]
        public void ShouldCreateNewProjectWhenNullIsPassedToLoadMethod()
        {
            _viewModel.Load(null);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(Guid.Empty, _viewModel.Project.ProjectId);
            Assert.Null(_viewModel.Project.ProjectNumber);
            Assert.Null(_viewModel.Project.ProjectClient);
            Assert.Equal("", _viewModel.Project.ProjectName);
            Assert.Null(_viewModel.Project.ProjectAddress);

            _projectRepositoryMock.Verify(pr => pr.GetProjectById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void ShouldEnableDeleteCommandForExistingProject()
        {
            _viewModel.Load(_projectId);

            Assert.True(_viewModel.DeleteProjectCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandForNewProject()
        {
            _viewModel.Load(null);

            Assert.False(_viewModel.DeleteProjectCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableDeleteCommandWithoudLoad()
        {
            Assert.False(_viewModel.DeleteProjectCommand.CanExecute(null));
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldCallDeleteMethodOfProjectRepositoryWhenDeleteProjectCommandIsExecuted(
            MessageDialogResult result, int expectedDeleteProjectCalls)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteProjectCommand.Execute(null);

            _projectRepositoryMock.Verify(pr => pr.DeleteProject(_projectId), 
                Times.Exactly(expectedDeleteProjectCalls));
            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        // TO DO: Find reason why this test fails. Same case as ShouldPublishOpenDetailViewEvent test.
        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldPublishProjectDeletedEventWhenDeleteProjectCommandIsExecuted(
            MessageDialogResult result, int expectedPublishCalls)
        {
            _viewModel.Load(_projectId);

            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            _viewModel.DeleteProjectCommand.Execute(null);

            var detailViewModelName = "ProjectDetailViewModel";
            _projectDeletedEventMock.Verify(e => e.Publish(
                new DetailDeletedEventArgs
                {
                    Id = _viewModel.Project.Model.ProjectId,
                    ViewModelName = detailViewModelName
                }), 
                Times.Exactly(expectedPublishCalls));

            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ShouldDisplayCorrectMessageInDeleteDialog()
        {
            _viewModel.Load(_projectId);

            var p = _viewModel.Project;
            p.ProjectClient = "Client";
            p.ProjectName = "ProjectName";

            _viewModel.DeleteProjectCommand.Execute(null);

            _messageDialogServiceMock.Verify(d => d.ShowYesNoDialog("Delete Project",
                $"Do you really want to delete the friend '{p.ProjectClient} {p.ProjectName}'?"),
                Times.Once);
        }
    }
}
