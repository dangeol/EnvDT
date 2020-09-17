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
        private Mock<IProjectMainViewModel> _projectMainViewModelMock;
        private Mock<ILabReportMainViewModel> _labReportMainViewModelMock;

        public MainViewModelTests()
        {
            _viewModelBase = new ViewModelBase();
            _projectMainViewModelMock = new Mock<IProjectMainViewModel>();
            _labReportMainViewModelMock = new Mock<ILabReportMainViewModel>();
            _viewModel = new MainViewModel(_projectMainViewModelMock.Object, _labReportMainViewModelMock.Object);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedViewModel()
        {
            var fired = _viewModel.IsPropertyChangedFired(() =>
            {
                _viewModel.CurrentViewModel = _viewModelBase;
            }, nameof(_viewModel.CurrentViewModel));

            Assert.True(fired);
        }
    }
}
