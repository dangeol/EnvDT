using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public interface IEvalCalc
    {
        public double SampleValueConversion(double sampleValue, string sampleValueUnitName, string refValUnitName);
        public bool IsSampleValueExceedingRefValue(double sampleValue, double refVal, string refValParamAnnot);
        public FinalSValue GetFinalSValue(
            EvalArgs evalArgs, string refValParamAnnot, List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs);
    }
}