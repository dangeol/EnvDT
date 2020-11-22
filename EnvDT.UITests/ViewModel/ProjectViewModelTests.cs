using EnvDT.Model.Entity;
using EnvDT.Model.IDataService;
using EnvDT.UI.Dialogs;
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
    public class ProjectViewModelTests
    {
        private ProjectViewModel _viewModel;
        private Mock<IProjectDetailViewModel> _projectDetailViewModelMock;

        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IMessageDialogService> _messageDialogServiceMock;
        private Mock<ILookupDataService> _lookupDataServiceMock;
        private OpenDetailViewEvent _openDetailViewEvent;
        private DetailSavedEvent _projectSavedEvent;
        private DetailDeletedEvent _detailDeletedEvent;
        private Project _project;
        private string _detailViewModelName;

        public ProjectViewModelTests()
        {
            _openDetailViewEvent = new OpenDetailViewEvent();
            _projectSavedEvent = new DetailSavedEvent();
            _detailDeletedEvent = new DetailDeletedEvent();
            _project = new Project();
            _detailViewModelName = "ProjectDetailViewModel";
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_openDetailViewEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailSavedEvent>())
                .Returns(_projectSavedEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailDeletedEvent>())
                .Returns(_detailDeletedEvent);

            _messageDialogServiceMock = new Mock<IMessageDialogService>();

            _lookupDataServiceMock = new Mock<ILookupDataService>();
            _lookupDataServiceMock.Setup(pr => pr.GetAllProjectsLookup())
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
            _viewModel = new ProjectViewModel(
                _lookupDataServiceMock.Object,
                _eventAggregatorMock.Object,
                CreateProjectDetailViewModel,
                _messageDialogServiceMock.Object);
        }

        private IProjectDetailViewModel CreateProjectDetailViewModel()
        {
            var projectDetailViewModelMock = new Mock<IProjectDetailViewModel>();
            projectDetailViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid?>(projectId =>
                {
                    _project.ProjectId = projectId.Value;
                    projectDetailViewModelMock.Setup(vm => vm.Project)
                    .Returns(new ProjectWrapper(_project));
                });
            _projectDetailViewModelMock = projectDetailViewModelMock;
            return projectDetailViewModelMock.Object;
        }

        [Fact]
        public void ShouldCallTheLoadMethod()
        {
            //2 Projects + 1 NavItemViewModelNull
            Assert.Equal(3, _viewModel.Projects.Count);
        }

        [Fact]
        public void ShouldLoadProjects()
        {
            _viewModel.LoadModels();

            Assert.Equal(3, _viewModel.Projects.Count);

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
        public void ShouldLoadProjectDetailViewModel()
        {
            var projectId = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = projectId,
                ViewModelName = _detailViewModelName
            });

            Assert.NotNull(_viewModel.DetailViewModel);
            var projectDetailVm = _viewModel.DetailViewModel;
            Assert.Equal(projectDetailVm, _viewModel.DetailViewModel);
            _projectDetailViewModelMock.Verify(vm => vm.Load(projectId), Times.Once);
        }

        [Fact]
        public void ShouldNotLoadProjectDetailViewModelWhenDefaultItemIsSelected()
        {
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = new Guid(),
                    ViewModelName = _detailViewModelName
                });
            Assert.Null(_viewModel.DetailViewModel);
        }

            [Fact]
        public void ShouldLoadProjectDetailViewModelAndLoadItWithIdNull()
        {
            Type type = typeof(ProjectDetailViewModel);
            _viewModel.CreateNewDetailCommand.Execute(parameter: type);

            Assert.NotNull(_viewModel.DetailViewModel);
            Assert.Equal(_projectDetailViewModelMock.Object, _viewModel.DetailViewModel);
            _projectDetailViewModelMock.Verify(vm => vm.Load(null), Times.Once);
        }

        [Fact]
        public void ShouldEnableProjectDetailViewWhenProjectSelected()
        {
            var projectId = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openDetailViewEvent.Publish(
                new OpenDetailViewEventArgs
                {
                    Id = projectId,
                    ViewModelName = _detailViewModelName
                });

            Assert.True(_viewModel.IsDetailViewEnabled);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedProject()
        {
            var projectDetailVmMock = new Mock<IProjectDetailViewModel>();
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.DetailViewModel = projectDetailVmMock.Object;
            }, nameof(_viewModel.DetailViewModel));

            Assert.True(fired);
        }

        [Fact]
        public void ShouldLoadProjectsOnlyOnce()
        {
            _viewModel.LoadModels();
            _viewModel.LoadModels();

            Assert.Equal(3, _viewModel.Projects.Count);
        }

        [Fact]
        public void ShouldUpdateProjectItemWhenProjectSaved()
        {
            _viewModel.LoadModels();

            var projectItem = _viewModel.Projects.First();

            var projectId = projectItem.LookupItemId;
            var projectNumber = "0123";
            var projectName = "ProName1Changed";

            _projectSavedEvent.Publish(
                new DetailSavedEventArgs
                {
                    Id = projectId,
                    DisplayMember = $"{projectNumber} {projectName}",
                    ViewModelName = nameof(ProjectDetailViewModel)
                });

            Assert.Equal("0123 ProName1Changed", projectItem.DisplayMember);
        }

        [Fact]
        public void ShouldAddProjectItemWhenAddedProjectIsSaved()
        {
            _viewModel.LoadModels();

            var newProjectId = new Guid("097e364a-5ef3-40a0-bde5-51caa26d7f48");
            var projectNumber = "8888";
            var projectName = "New Project";

            _projectSavedEvent.Publish(
                new DetailSavedEventArgs
                {
                    Id = newProjectId,
                    DisplayMember = $"{projectNumber} {projectName}",
                    ViewModelName = nameof(ProjectDetailViewModel)
                });

            Assert.Equal(4, _viewModel.Projects.Count);

            var addDetailItem = _viewModel.Projects.SingleOrDefault(p => p.LookupItemId == newProjectId);
            Assert.NotNull(addDetailItem);
            Assert.Equal("8888 New Project", addDetailItem.DisplayMember);
        }

        [Fact]
        public void ShouldRemoveProjectItemWhenProjectIsDeleted()
        {
            _viewModel.LoadModels();

            var existingProjectId = _viewModel.Projects.First().LookupItemId;

            _detailDeletedEvent.Publish(
            new DetailDeletedEventArgs
            {
                Id = existingProjectId,
                ViewModelName = _detailViewModelName
            });

            Assert.Equal(2, _viewModel.Projects.Count);

            var deletDetailem = _viewModel.Projects.SingleOrDefault(p => p.LookupItemId == existingProjectId);
            Assert.Null(deletDetailem);
        }

        [Fact]
        public void ShouldRemoveDetailViewModelOnDeletedEvent()
        {
            var deletedProjectId = Guid.Parse("8abbae81-ad7e-4453-8546-1278b625c50f");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = Guid.Parse("d24af541-2cda-4580-bbb1-675b41f16dea"),
                ViewModelName = _detailViewModelName
            });
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = Guid.Parse("2f9f2fa5-9db0-4aae-ba28-113e282e65c5"),
                ViewModelName = _detailViewModelName
            });

            _detailDeletedEvent.Publish(
            new DetailDeletedEventArgs
            {
                Id = deletedProjectId,
                ViewModelName = _detailViewModelName
            });

            Assert.Null(_viewModel.DetailViewModel);
            Assert.False(_viewModel.IsDetailViewEnabled);
        }

        [Theory]
        [InlineData(MessageDialogResult.Yes, 1)]
        [InlineData(MessageDialogResult.No, 0)]
        public void ShouldNotLoadNewProjectDetailViewModelWhenUnsavedChangesLeft(
            MessageDialogResult result, int projectViewLoaded)
        {    
            _messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>())).Returns(result);

            var projectId1 = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = projectId1,
                ViewModelName = _detailViewModelName
            });

            _projectDetailViewModelMock.Setup(vm => vm.HasChanges).Returns(true);

            var projectId2 = new Guid("aa4ec543-065e-41c5-8324-ccb39d071d0b");
            _openDetailViewEvent.Publish(
            new OpenDetailViewEventArgs
            {
                Id = projectId2,
                ViewModelName = _detailViewModelName
            });

            _projectDetailViewModelMock.Verify(vm => vm.Load(projectId2), Times.Exactly(projectViewLoaded));
            _messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}

