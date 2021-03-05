using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.Model.Core
{
    public class EvalCalc : IEvalCalc
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

        public bool IsSampleValueExceedingRefValue(double sampleValue, double refVal, string refValParamAnnot)
        {
            return refValParamAnnot != "lower" && sampleValue > refVal
                   || refValParamAnnot == "lower" && sampleValue < refVal;
        }

        public FinalSValue GetFinalSValue(EvalArgs evalArgs, string refValParamAnnot, List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs)
        {
            // get min or max sample value of same final parameter, but from different substrate
            var sValuesWithParamName = LrParamSValuePairs.GroupBy(sv => sv.Key.LabReportParamName).Select(g => new
            {
                g.Key,
                Value = evalArgs.SelectSameLrParamMaxValue ? g.Max(row => row.Value) : g.Min(row => row.Value)
            });

            // finally, get min or max sample value of same final parameter with different analytical methods.
            var finalValueWithParamName = evalArgs.SelectDiffLrParamMaxValue ||
                    // for e.g. pH-Values, always take the max value
                    refValParamAnnot.Equals("lower") || refValParamAnnot.Equals("upper") ?
                sValuesWithParamName.Aggregate((l, r) => l.Value > r.Value ? l : r) :
                sValuesWithParamName.Aggregate((l, r) => l.Value < r.Value ? l : r);

            var labReportParamName = sValuesWithParamName.Count() > 1 
                && !evalArgs.SelectDiffLrParamMaxValue 
                && !refValParamAnnot.Equals("lower") 
                && !refValParamAnnot.Equals("upper") ?
                    finalValueWithParamName.Key : "";

            double sValue = finalValueWithParamName.Value;
            FinalSValue finalSValue = new FinalSValue
            {
                LabReportParamName = labReportParamName,
                SValue = finalValueWithParamName.Value
            };
            return finalSValue;
        }
    }
}
