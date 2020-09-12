using EnvDT.Model;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using Moq;
using Prism.Events;
using System;
using Xunit;
using EnvDT.UITests.Extensions;
using EnvDT.UI.Wrapper;

namespace EnvDT.UITests.ViewModel
{
    public class MainViewModelTests
    {
        private Mock<IProjectMainViewModel> _projectMainViewModelMock;
        private OpenProjectEditViewEvent _openProjectEditViewEvent;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private MainViewModel _viewModel;
        private Mock<IProjectEditViewModel> _projectEditViewModelMock;

        public MainViewModelTests()
        {
            _projectMainViewModelMock = new Mock<IProjectMainViewModel>();

            _openProjectEditViewEvent = new OpenProjectEditViewEvent();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenProjectEditViewEvent>())
                .Returns(_openProjectEditViewEvent);

            _viewModel = new MainViewModel(_projectMainViewModelMock.Object,
                CreateProjectEditViewModel, _eventAggregatorMock.Object);

        }

        private IProjectEditViewModel CreateProjectEditViewModel()
        {
            var projectEditViewModelMock = new Mock<IProjectEditViewModel>();
            projectEditViewModelMock.Setup(vm => vm.Load(It.IsAny<Guid>()))
                .Callback<Guid>(projectId =>
                {
                    projectEditViewModelMock.Setup(vm => vm.Project)
                    .Returns(new ProjectWrapper(new Project { ProjectId = projectId }));
                });
            _projectEditViewModelMock = projectEditViewModelMock;
            return projectEditViewModelMock.Object;
        }


        [Fact]
        public void ShouldLoadProjectEditViewModel()
        {
            var projectId = new Guid("891d2d54-e1ad-4431-ab22-8e0899f08a14");
            _openProjectEditViewEvent.Publish(projectId);

            Assert.NotNull(_viewModel.ProjectEditViewModel);
            var projectEditVm = _viewModel.ProjectEditViewModel;

            _projectEditViewModelMock.Verify(vm => vm.Load(projectId), Times.Once);
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
    }
}
