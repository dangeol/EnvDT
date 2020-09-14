using EnvDT.UI.ViewModel;
using Moq;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainViewModelTests
    {
        private Mock<IProjectMainViewModel> _projectMainViewModelMock;
        private MainViewModel _viewModel;

        public MainViewModelTests()
        {
            _projectMainViewModelMock = new Mock<IProjectMainViewModel>();
            _viewModel = new MainViewModel(_projectMainViewModelMock.Object);

        }

        [Fact]
        public void ShouldCallTheLoadMethodOfTheProjectMainViewModel()
        {
            _viewModel.Load(); 

            _projectMainViewModelMock.Verify(vm => vm.LoadProjects(), Times.Once);
        }
    }
}
