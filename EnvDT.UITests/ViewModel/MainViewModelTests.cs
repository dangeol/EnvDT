using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private Mock<OpenDetailViewEvent> _openDetailViewEventMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IProjectViewModel> _projectViewModelMock;
        private Mock<Func<ISampleDetailViewModel>> _sampleDetailVmCreatorMock;
        private IMenuViewModel _mainTabViewModel;
        private Mock<IMainTabViewModel> _mainTabViewModelMock;
        private Mock<ISettingsDetailViewModel> _settingsDetailViewModelMock;

        public MainViewModelTests()
        {
            _openDetailViewEventMock = new Mock<OpenDetailViewEvent>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_openDetailViewEventMock.Object);
            _projectViewModelMock = new Mock<IProjectViewModel>();
            _sampleDetailVmCreatorMock = new Mock<Func<ISampleDetailViewModel>>();
            _mainTabViewModel = new MainTabViewModel(_eventAggregatorMock.Object,
                _projectViewModelMock.Object, _sampleDetailVmCreatorMock.Object);
            _mainTabViewModelMock = new Mock<IMainTabViewModel>();
            _settingsDetailViewModelMock = new Mock<ISettingsDetailViewModel>();

            _viewModel = new MainViewModel(_mainTabViewModelMock.Object, _settingsDetailViewModelMock.Object);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedViewModel()
        {
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.CurrentViewModel = _mainTabViewModel;
            }, nameof(_viewModel.CurrentViewModel));

            Assert.True(fired);
        }
    }
}
