using EnvDT.UI.Data.Repository;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectEditViewModelTests
    {
        private Guid _projectId = new Guid("77ec605f-3909-471f-a866-a2c4759bf5a0");
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private ProjectEditViewModel _viewModel;
        private Mock<ProjectSavedEvent> _projectSavedEventMock;
        private Mock<ProjectDeletedEvent> _projectDeletedEventMock;

        public ProjectEditViewModelTests()
        {
            _projectSavedEventMock = new Mock<ProjectSavedEvent>();
            _projectDeletedEventMock = new Mock<ProjectDeletedEvent>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(pr => pr.GetProjectById(_projectId))
                .Returns(new Model.Project { ProjectId = _projectId, ProjectNumber = "012345" });
            _eventAggregatorMock = new Mock<IEventAggregator>(); 
            _eventAggregatorMock.Setup(ea => ea.GetEvent<ProjectSavedEvent>())
                .Returns(_projectSavedEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<ProjectDeletedEvent>())
                .Returns(_projectDeletedEventMock.Object);

            _viewModel = new ProjectEditViewModel(_projectRepositoryMock.Object, _eventAggregatorMock.Object);
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
        public void ShouldEnableSaveProjectCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

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
            _viewModel.Project.AcceptChanges();
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
            _projectRepositoryMock.Verify(pr => pr.SaveProject(_viewModel.Project.Model), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveProjectCommand.Execute(null);
            Assert.False(_viewModel.Project.IsChanged);
        }

        [Fact]
        public void ShouldPublishProjectSavedEventWhenSaveProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveProjectCommand.Execute(null);

            _projectSavedEventMock.Verify(e => e.Publish(_viewModel.Project.Model), Times.Once);
        }

        [Fact]
        public void ShouldCreateNewProjectWhenNullIsPassedToLoadMethod()
        {
            _viewModel.Load(null);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(Guid.Empty, _viewModel.Project.ProjectId);
            Assert.Null(_viewModel.Project.ProjectNumber);
            Assert.Null(_viewModel.Project.ProjectClient);
            Assert.Null(_viewModel.Project.ProjectName);
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

        [Fact]
        public void ShouldCallDeleteMethodOfProjectRepositoryWhenDeleteProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);

            _viewModel.DeleteProjectCommand.Execute(null);
            _projectRepositoryMock.Verify(pr => pr.DeleteProject(_projectId), Times.Once);
        }

        [Fact]
        public void ShouldPublishProjectDeletedEventWhenDeleteProjectCommandIsExecuted()
        {
            _viewModel.Load(_projectId);

            _viewModel.DeleteProjectCommand.Execute(null);
            _projectDeletedEventMock.Verify(e => e.Publish(_viewModel.Project.Model.ProjectId), Times.Once);
        }
    }
}
