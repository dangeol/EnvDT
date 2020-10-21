namespace EnvDT.Model.Core
{
    public interface IEvalCalcService
    {
        public double SampleValueConversion(double sampleValue, string sampleValueUnitName, string refValUnitName);
        public bool IsSampleValueExceedingRefValue(double sampleValue, double refVal, string refValParamAnnot);
    }
}