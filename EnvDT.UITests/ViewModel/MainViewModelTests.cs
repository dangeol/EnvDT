using EnvDT.UI.ViewModel;
using Xunit;

namespace EnvDT.UITests.ViewModel
{
    public class MainViewModelTests
    {
        [Fact]
        public void ShouldCallTheLoadMethodOfTheNavigationViewModel()
        {
            var navigationViewModelMock = new NavigationViewModelMock();
            var viewModel = new MainViewModel(navigationViewModelMock);
            
            viewModel.Load();

            Assert.True(navigationViewModelMock.LoadProjectsHasBeenCalled);
        }
        public class NavigationViewModelMock : INavigationViewModel
        {
            public bool LoadProjectsHasBeenCalled { get; set; }
            public void LoadProjects()
            {
                LoadProjectsHasBeenCalled = true;
            }

        }
    }
}
