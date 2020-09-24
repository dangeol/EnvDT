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
        private Mock<IProjectViewModel> _projectViewModelMock;
        private Mock<IEvalViewModel> _labReportMainViewModelMock;

        public MainViewModelTests()
        {
            _viewModelBase = new ViewModelBase();
            _projectViewModelMock = new Mock<IProjectViewModel>();
            _labReportMainViewModelMock = new Mock<IEvalViewModel>();
            _viewModel = new MainViewModel(_projectViewModelMock.Object, _labReportMainViewModelMock.Object);
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
