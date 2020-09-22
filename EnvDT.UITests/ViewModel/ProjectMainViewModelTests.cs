using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UI.Wrapper;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class ProjectMainViewModelTests
    {
        private ProjectMainViewModel _viewModel;
        private Mock<IProjectEditViewModel> _projectEditViewModelMock;

        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private OpenProjectEditViewEvent _openProjectEditViewEvent;
        private ProjectSavedEvent _projectSavedEvent;
        private ProjectDeletedEvent _projectDeletedEvent;
        private Project _project;

        public ProjectMainViewModelTests()
        {
            _openProjectEditViewEvent = new OpenProjectEditViewEvent();
            _projectSavedEvent = new ProjectSavedEvent();
            _projectDeletedEvent = new ProjectDeletedEvent();
            _project = new Project();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenProjectEditViewEvent>())
                .Returns(_openProjectEditViewEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<ProjectSavedEvent>())
                .Returns(_projectSavedEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<ProjectDeletedEvent>())
                .Returns(_projectDeletedEvent);

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock.Setup(pr => pr.GetAllProjects())
                .Returns(new List<LookupItem>
                {
                    new LookupItem
                    {
                        LookupItemId = new Guid("67455421-0498-46af-9241-7287539fcade"),
                        DisplayMember = "111.111 MockProject1"
                    },
                    new LookupItem
                    {
                        LookupItemId = new Guid("13ce3bee-d343-4851-81a8-ce916f6756db"),
                        DisplayMember = "222.222 MockProject2"
                    }
                });
            _viewModel = new ProjectMainViewModel(
                projectRepositoryMock.Object,
                _eventAggregatorMock.Object,
                CreateProjectEditViewModel,
                _messageDialogServiceMock.Object);
        }

        private IProjectEditViewModel CreateProjectEditViewModel()
        {
            var projectEditViewModelMock = new Mock<IProjectEditViewModel>();
            projectEditViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(projectId =>
                {
                    _project.ProjectId = projectId.Value;
                    projectEditViewModelMock.Setup(vm => vm.Project)
                    .Returns(new ProjectWrapper(_project));
                });
            _projectEditViewModelMock = projectEditViewModelMock;
            return projectEditViewModelMock.Object;
        }

        [Fact]
        public void ShouldCallTheLoadMethod()
        {
            Assert.Equal(2, _viewModel.Projects.Count);
        }

        [Fact]
        public void ShouldLoadProjects()
        {
            _viewModel.LoadProjects();

            Assert.Equal(2, _viewModel.Projects.Count);

            var project = _viewModel.Projects.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("67455421-0498-46af-9241-7287539fcade"));
            Assert.NotNull(project);
            Assert.Equal("111.111 MockProject1", project.DisplayMember);

            project = _viewModel.Projects.SingleOrDefault(
                p => p.LookupItemId == Guid.Parse("13ce3bee-d343-4851-81a8-ce916f6756db"));
            Assert.NotNull(project);
            Assert.Equal("222.222 MockProject2", project.DisplayMember);
        }

        [Fact]
        public void ShouldLoadProjectEditViewModel()
        {
            var projectId = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openProjectEditViewEvent.Publish(projectId);

            Assert.NotNull(_viewModel.ProjectEditViewModel);
            var projectEditVm = _viewModel.ProjectEditViewModel;
            Assert.Equal(projectEditVm, _viewModel.ProjectEditViewModel);
            _projectEditViewModelMock.Verify(vm => vm.Load(projectId), Times.Once);
        }

        [Fact]
        public void ShouldLoadProjectEditViewModelAndLoadItWithIdNull()
        {
            _viewModel.AddProjectCommand.Execute(null);

            Assert.NotNull(_viewModel.ProjectEditViewModel);
            var projectEditVm = _viewModel.ProjectEditViewModel;
            Assert.Equal(projectEditVm, _viewModel.ProjectEditViewModel);
            _projectEditViewModelMock.Verify(vm => vm.Load(null), Times.Once);
        }

        [Fact]
        public void ShouldEnableProjectEditViewWhenProjectSelected()
        {
            var projectId = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openProjectEditViewEvent.Publish(projectId);

            Assert.True(_viewModel.IsProjectEditViewEnabled);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedProject()
        {
            var projectEditVmMock = new Mock<IProjectEditViewModel>();
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.ProjectEditViewModel = projectEditVmMock.Object;
            }, nameof(_viewModel.ProjectEditViewModel));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldLoadProjectsOnlyOnce()
        {
            _viewModel.LoadProjects();
            _viewModel.LoadProjects();

            Assert.Equal(2, _viewModel.Projects.Count);
        }

        [Fact]
        public void ShouldUpdateProjectItemWhenProjectSaved()
        {
            _viewModel.LoadProjects();

            var projectItem = _viewModel.Projects.First();

            var projectId = projectItem.LookupItemId;

            _projectSavedEvent.Publish(
                new Project
                {
                    ProjectId = projectId,
                    ProjectNumber = "0123",
                    ProjectName = "ProName1Changed",
                });

            Assert.Equal("0123 ProName1Changed", projectItem.DisplayMember);
        }

        [Fact]
        public void ShouldAddProjectItemWhenAddedProjectIsSaved()
        {
            _viewModel.LoadProjects();

            var newProjectId = new Guid("097e364a-5ef3-40a0-bde5-51caa26d7f48");

            _projectSavedEvent.Publish(
                new Project
                {
                    ProjectId = newProjectId,
                    ProjectNumber = "8888",
                    ProjectName = "New Project",
                });

            Assert.Equal(3, _viewModel.Projects.Count);

            var addedItem = _viewModel.Projects.SingleOrDefault(p => p.LookupItemId == newProjectId);
            Assert.NotNull(addedItem);
            Assert.Equal("8888 New Project", addedItem.DisplayMember);
        }

        [Fact]
        public void ShouldRemoveProjectItemWhenProjectIsDeleted()
        {
            _viewModel.LoadProjects();

            var existingProjectId = _viewModel.Projects.First().LookupItemId;

            _projectDeletedEvent.Publish(existingProjectId);

            Assert.Single(_viewModel.Projects);

            var deletedItem = _viewModel.Projects.SingleOrDefault(p => p.LookupItemId == existingProjectId);
            Assert.Null(deletedItem);
        }

        [Fact]
        public void ShouldRemoveProjectEditViewModelOnProjectDeletedEvent()
        {
            var deletedProjectId = Guid.Parse("8abbae81-ad7e-4453-8546-1278b625c50f");
            _openProjectEditViewEvent.Publish(Guid.Parse("d24af541-2cda-4580-bbb1-675b41f16dea"));
            _openProjectEditViewEvent.Publish(Guid.Parse("2f9f2fa5-9db0-4aae-ba28-113e282e65c5"));
            _openProjectEditViewEvent.Publish(deletedProjectId);

            _projectDeletedEvent.Publish(deletedProjectId);

            Assert.Null(_viewModel.ProjectEditViewModel.Project);
            Assert.False(_viewModel.IsProjectEditViewEnabled);
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldNotLoadNewProjectEditViewModelWhenUnsavedChangesLeft(
            MessageDialogResult result, int projectViewLoaded)
        {    
            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            var projectId1 = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openProjectEditViewEvent.Publish(projectId1);

            _projectEditViewModelMock.Setup(vm => vm.HasChanges).Returns(true);

            var projectId2 = new Guid("aa4ec543-065e-41c5-8324-ccb39d071d0b");
            _openProjectEditViewEvent.Publish(projectId2);

            _projectEditViewModelMock.Verify(vm => vm.Load(projectId2), Times.Exactly(projectViewLoaded));
            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}

