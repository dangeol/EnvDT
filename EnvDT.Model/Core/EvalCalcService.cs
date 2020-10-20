namespace EnvDT.Model.Core
{
    public class EvalCalcService : IEvalCalcService
    {
       public double SampleValueConversion(double sampleValue, string sampleValueUnitName, string refValUnitName)
        {
            if (refValUnitName.Length > 0 && refValUnitName.Substring(0, 1) ==
                "m" && sampleValueUnitName.Substring(0, 1) == "µ")
                sampleValue /= 1000;
            else if (refValUnitName.Length > 0 && refValUnitName.Substring(0, 1) ==
                "µ" && sampleValueUnitName.Substring(0, 1) == "m")
                sampleValue *= 1000;
            return sampleValue;
        }
    }
}
