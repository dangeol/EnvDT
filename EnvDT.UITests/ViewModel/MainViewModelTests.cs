using EnvDT.UI.Event;
using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private Mock<OpenDetailViewEvent> _openDetailViewEventMock;
        private Mock<DetailClosedEvent> _detailClosedEventMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private Mock<IProjectViewModel> _projectViewModelMock;
        private Mock<ITab> _tab;
        private Mock<Func<ISampleDetailViewModel>> _sampleDetailVmCreatorMock;
        private IMenuViewModel _mainTabViewModel;
        private Mock<IMainTabViewModel> _mainTabViewModelMock;
        private Mock<ILabViewModel> _labViewModelMock;
        private Mock<ISettingsDetailViewModel> _settingsDetailViewModelMock;
        private Mock<IInfoDetailViewModel> _infoDetailViewModelMock;

        public MainViewModelTests()
        {
            _openDetailViewEventMock = new Mock<OpenDetailViewEvent>();
            _detailClosedEventMock = new Mock<DetailClosedEvent>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenDetailViewEvent>())
                .Returns(_openDetailViewEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<DetailClosedEvent>())
                .Returns(_detailClosedEventMock.Object);
            _projectViewModelMock = new Mock<IProjectViewModel>();
            _tab = new Mock<ITab>();
            _tab.Setup(t => t.TabbedViewModels).Returns(new ObservableCollection<IMainTabViewModel>());
            _sampleDetailVmCreatorMock = new Mock<Func<ISampleDetailViewModel>>();
            _mainTabViewModel = new MainTabViewModel(_eventAggregatorMock.Object, _tab.Object,
                _projectViewModelMock.Object, _sampleDetailVmCreatorMock.Object);
            _mainTabViewModelMock = new Mock<IMainTabViewModel>();
            _labViewModelMock = new Mock<ILabViewModel>();
            _settingsDetailViewModelMock = new Mock<ISettingsDetailViewModel>();
            _infoDetailViewModelMock = new Mock<IInfoDetailViewModel>();

            _viewModel = new MainViewModel(_mainTabViewModelMock.Object, _labViewModelMock.Object,
                _settingsDetailViewModelMock.Object, _infoDetailViewModelMock.Object);
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
