using EnvDT.UI.Wrapper;

namespace EnvDT.UI.ViewModel
{
    public interface IProjectDetailViewModel : IDetailViewModel
    {
        ProjectWrapper Project { get; }
    }
}
