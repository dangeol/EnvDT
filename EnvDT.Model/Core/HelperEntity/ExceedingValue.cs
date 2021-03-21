namespace EnvDT.Model.Core.HelperEntity
{
    public class ExceedingValue
    {
        public int Level { get; set; }
        public string ParamName { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public bool IsNotExclusionCriterion { get; set; }
    }
}
