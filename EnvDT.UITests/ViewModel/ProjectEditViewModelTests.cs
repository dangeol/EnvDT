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
        private NavigationViewModel _navigationViewModelMock;
        private Mock<ProjectSavedEvent> _projectSavedEventMock;


        public ProjectEditViewModelTests()
        {
            _projectSavedEventMock = new Mock<ProjectSavedEvent>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(pr => pr.GetProjectById(_projectId))
                .Returns(new Model.Project { ProjectId = _projectId, ProjectNumber = "012345" });
            _eventAggregatorMock = new Mock<IEventAggregator>(); _eventAggregatorMock
                .Setup(ea => ea.GetEvent<ProjectSavedEvent>())
                .Returns(_projectSavedEventMock.Object);

            _navigationViewModelMock = new NavigationViewModel(_projectRepositoryMock.Object, _eventAggregatorMock.Object);
            _viewModel = new ProjectEditViewModel(_projectRepositoryMock.Object, _eventAggregatorMock.Object, _navigationViewModelMock);
        }

        [Fact]
        public void ShouldLoadProject()
        {
            _viewModel.Load(_projectId);

            Assert.NotNull(_viewModel.Project);
            Assert.Equal(_projectId, _viewModel.Project.ProjectId);

            _projectRepositoryMock.Verify(pr => pr.GetProjectById(_projectId), Times.Once);
        }


        /* TO DO
        [Fact]
        public void ShouldCallTheLoadMethodOfTheNavigationViewModel()
        {
            _navigationViewModelMock.Verify(vm => vm.LoadProjects(), Times.Once);
        }
        */

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
        public void ShouldEnableSaveCommandWhenProjectIsChanged()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void ShouldDisableSaveCommandWithoutLoad()
        {
            Assert.False(_viewModel.SaveCommand.CanExecute(null));
        }

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
        public void ShouldCallSaveMethodOfProjectRepositoryWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _projectRepositoryMock.Verify(pr => pr.SaveProject(_viewModel.Project.Model), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);
            Assert.False(_viewModel.Project.IsChanged);
        }

        [Fact]
        public void ShouldPublishProjectSavedEventWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_projectId);
            _viewModel.Project.ProjectNumber = "Changed";

            _viewModel.SaveCommand.Execute(null);

            _projectSavedEventMock.Verify(e => e.Publish(_viewModel.Project.Model), Times.Once);
        }
    }
}
