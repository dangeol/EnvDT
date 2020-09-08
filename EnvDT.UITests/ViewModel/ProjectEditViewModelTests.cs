using EnvDT.UI.Data.Repository;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using System;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectEditViewModelTests
    {
        private Guid _projectId = new Guid("77ec605f-3909-471f-a866-a2c4759bf5a0");
        private Mock<IProjectRepository> _projectRepositoryMock;
        private ProjectEditViewModel _viewModel;
    

        public ProjectEditViewModelTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(pr => pr.GetProjectById(_projectId))
                .Returns(new Model.Project { ProjectId = _projectId, ProjectNumber = "012345" });

            _viewModel = new ProjectEditViewModel(_projectRepositoryMock.Object);
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
    }
}
