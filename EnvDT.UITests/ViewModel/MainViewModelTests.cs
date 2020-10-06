using EnvDT.UI.ViewModel;
using EnvDT.UITests.Extensions;
using Moq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainViewModelTests
    {
        private MainViewModel _viewModel;
        private ViewModelBase _viewModelBase;
        private Mock<MainTabViewModel> _mainTabViewModelMock;
        private Mock<ISettingsDetailViewModel> _settingsDetailViewModelMock;

        public MainViewModelTests()
        {
            _viewModelBase = new ViewModelBase();
            _mainTabViewModelMock = new Mock<MainTabViewModel>();
            _settingsDetailViewModelMock = new Mock<ISettingsDetailViewModel>();
            _viewModel = new MainViewModel(_mainTabViewModelMock.Object, _settingsDetailViewModelMock.Object);
        }

        /* TO DO: update test case
        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedViewModel()
        {
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel CurrentViewModel = _viewModelBase;
            }, nameof(_viewModel.CurrentViewModel));

            Assert.True(fired);
        }
        */
    }
}
