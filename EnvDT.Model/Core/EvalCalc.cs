using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.Model.Core
{
    public class EvalCalc : IEvalCalc
    {
        private readonly IUnitOfWork _unitOfWork;

        public EvalCalc(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public double SampleValueConversion(double sampleValue, string sampleValueUnitName, string refValUnitName)
        {
            if (refValUnitName.Length > 0 && refValUnitName[..1] ==
                "m" && sampleValueUnitName[..1] == "µ")
                sampleValue /= 1000;
            else if (refValUnitName.Length > 0 && refValUnitName[..1] ==
                "µ" && sampleValueUnitName[..1] == "m")
                sampleValue *= 1000;
            return sampleValue;
        }

        public bool IsSampleValueExceedingRefValue(double sampleValue, double refVal, string refValParamAnnot)
        {
            return refValParamAnnot != "lower" && sampleValue > refVal
                   || refValParamAnnot == "lower" && sampleValue < refVal;
        }

        public List<KeyValuePair<LabReportParam, double>> GetLrParamSValuePairs(
            IEnumerable<LabReportParam> labReportParams, string refValUnitName)
        {
            List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs = new();

            foreach (LabReportParam lrparam in labReportParams)
            {
                var sampleValueUnitName = _unitOfWork.Units.GetById(lrparam.UnitId).UnitName;
                var sValuesFromLrParam = _unitOfWork.SampleValues.GetSampleValuesByLabReportParamId(lrparam.LabReportParamId);

                foreach (SampleValue sValueFromLrParam in sValuesFromLrParam)
                {
                    var sValueConverted = SampleValueConversion(sValueFromLrParam.SValue, sampleValueUnitName, refValUnitName);
                    LrParamSValuePairs.Add(new KeyValuePair<LabReportParam, double>(lrparam, sValueConverted));
                }
            }

            return LrParamSValuePairs;
        }

        public List<KeyValuePair<LabReportParam, double>> GetLrParamSValuePairs(
            IEnumerable<LabReportParam> labReportParams, Guid sampleId, string refValUnitName)
        {
            List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs = new();

            foreach (LabReportParam lrparam in labReportParams)
            {
                var sampleValueUnitName = _unitOfWork.Units.GetById(lrparam.UnitId).UnitName;
                var sValuesFromLrParam = _unitOfWork.SampleValues.GetSampleValuesBySampleIdAndLabReportParamId(
                                            sampleId, lrparam.LabReportParamId);

                foreach (SampleValue sValueFromLrParam in sValuesFromLrParam)
                {
                    var sValueConverted = SampleValueConversion(sValueFromLrParam.SValue, sampleValueUnitName, refValUnitName);
                    LrParamSValuePairs.Add(new KeyValuePair<LabReportParam, double>(lrparam, sValueConverted));
                }
            }

            return LrParamSValuePairs;
        }

        public FinalSValue GetFinalSValue(EvalArgs evalArgs, string refValParamAnnot, List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs)
        {
            // get min or max sample value of same final parameter, but from different sample fraction
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
            FinalSValue finalSValue = new()
            {
                LabReportParamName = labReportParamName,
                SValue = finalValueWithParamName.Value
            };
            return finalSValue;
        }
    }
}
