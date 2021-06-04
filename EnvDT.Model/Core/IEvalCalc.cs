using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public interface IEvalCalc
    {
        public double SampleValueConversion(double sampleValue, string sampleValueUnitName, string refValUnitName);
        public bool IsSampleValueExceedingRefValue(double sampleValue, double refVal, string refValParamAnnot);
        public List<KeyValuePair<LabReportParam, double>> GetLrParamSValuePairs(
            IEnumerable<LabReportParam> labReportParams, string refValUnitName);
        public List<KeyValuePair<LabReportParam, double>> GetLrParamSValuePairs(
            IEnumerable<LabReportParam> labReportParams, Guid sampleId, string refValUnitName);
        public FinalSValue GetFinalSValue(
            EvalArgs evalArgs, string refValParamAnnot, List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs);
    }
}