namespace EnvDT.Model.Core.HelperEntity
{
    public class ExceedingValue
    {
        public bool IsExceeding { get; set; }
        public bool IsBetweenRefValueRefValAlt { get; set; }
        public int Level { get; set; }
        public bool IsGroupClass { get; set; }
        public string ParamName { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public bool IsNotExclusionCriterion { get; set; }
    }
}
