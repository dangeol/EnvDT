using EnvDT.Model.Core.HelperEntity;

namespace EnvDT.UI.Service
{
    public interface IReadFileHelper
    {
        public ImportedFileData ReadFile(string file);
    }
}
