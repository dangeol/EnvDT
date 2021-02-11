using EnvDT.UI.Wrapper;

namespace EnvDT.UI.ViewModel
{
    public interface IConfigCsvDetailViewModel : IDetailViewModel
    {
        ConfigCsvWrapper ConfigCsv { get; }
    }
}
