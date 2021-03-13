using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.Core.HelperEntity
{
    public class FootnoteSValueList
    {
        public Dictionary<string, double> FinalSValues { get; set; }
        public HashSet<PublParam> MissingParams { get; set; }
    }
}
