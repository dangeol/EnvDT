using EnvDT.UI.Wrapper;

namespace EnvDT.UI.ViewModel
{
    public interface IConfigXlsxDetailViewModel : IDetailViewModel
    {
        ConfigXlsxWrapper ConfigXlsx { get; }
    }
}
