using EnvDT.UI.Wrapper;

namespace EnvDT.UI.ViewModel
{
    public interface IConfigXmlDetailViewModel : IDetailViewModel
    {
        ConfigXmlWrapper ConfigXml { get; }
    }
}
