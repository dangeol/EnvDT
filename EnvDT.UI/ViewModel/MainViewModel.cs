using System;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase

    {
        public MainViewModel(IProjectMainViewModel projectMainViewModel)
        {
            ProjectMainViewModel = projectMainViewModel;
        }

        public void Load()
        {
            ProjectMainViewModel.LoadProjects();
        }

        public IProjectMainViewModel ProjectMainViewModel { get; private set; }
    }
}
