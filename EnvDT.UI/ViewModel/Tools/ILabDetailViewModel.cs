using EnvDT.UI.Wrapper;

namespace EnvDT.UI.ViewModel
{
    public interface ILabDetailViewModel : IDetailViewModel
    {
        LabWrapper Laboratory { get; }
    }
}
