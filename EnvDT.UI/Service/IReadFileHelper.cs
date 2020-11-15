using System.Data;

namespace EnvDT.UI.Service
{
    public interface IReadFileHelper
    {
        public DataTable ReadFile(string file);
    }
}
