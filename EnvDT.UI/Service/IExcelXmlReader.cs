using System.Data;
using System.IO;

namespace EnvDT.UI.Service
{
    public interface IExcelXmlReader
    {
        public DataSet ReadExcelXml(Stream stream);
    }
}
