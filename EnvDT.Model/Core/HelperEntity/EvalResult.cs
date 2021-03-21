using System.Collections.Generic;

namespace EnvDT.Model.Core.HelperEntity
{
    public class EvalResult
    {
        public string SampleName { get; set; }
        public string HighestValClassName { get; set; }
        public string ExceedingValues { get; set; }
        public HashSet<string> GeneralFootnoteTexts { get; set; }
        public string MissingParams { get; set; }
        public string MinValueParams { get; set; }
        public string TakingAccountOf { get; set; }
    }
}
