using EnvDT.Model.Entity;
using EnvDT.Model.Core.HelperEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using EnvDT.Model.IRepository;

namespace EnvDT.Model.Core
{
    public class EvalLabReportService : IEvalLabReportService
    {   
        private IUnitOfWork _unitOfWork;
        private ILabReportPreCheck _labReportPreCheck;
        private IEvalCalc _evalCalc;
        private EvalResult _evalResult;
        private HashSet<PublParam> _missingParams = new HashSet<PublParam>();
        private HashSet<string> _paramNamesForMin = new HashSet<string>();

        public EvalLabReportService(IUnitOfWork unitOfWork, ILabReportPreCheck labReportPreCheck, IEvalCalc evalCalc)
        {
            _unitOfWork = unitOfWork;
            _labReportPreCheck = labReportPreCheck;
            _evalCalc = evalCalc;
        }

        public bool LabReportPreCheck(Guid labReportId, IReadOnlyCollection<Guid> publicationIds)
        {
            // TO DO: refactor - find synergies with GetEvalResult method to increase efficiency
            return _labReportPreCheck.FindMissingParametersUnits(labReportId, publicationIds);
        }

        public EvalResult GetEvalResult(EvalArgs evalArgs)
        {
            _missingParams.Clear();
            _paramNamesForMin.Clear();
            _evalResult = new EvalResult();
            var publication = _unitOfWork.Publications.GetById(evalArgs.PublicationId);
            var publParams = publication.PublParams;
            var highestLevel = 0;
            List<ExceedingValue> exceedingValues = new List<ExceedingValue>();

            foreach (PublParam publParam in publParams)
            {
                var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam, evalArgs.LabReportId);

                if (labReportParams.Count() == 0)
                {
                    if (publParam.IsMandatory)
                    {
                        _missingParams.Add(publParam);
                    }                    
                    continue;
                }

                var refValues = Enumerable.Empty<RefValue>();

                if (publication.UsesMediumSubTypes && !publication.UsesConditions)
                {
                    refValues = _unitOfWork.RefValues.GetRefValuesWithMedSubTypesByPublParamIdAndSample(
                        publParam.PublParamId, evalArgs.Sample);
                }
                else if (!publication.UsesMediumSubTypes && publication.UsesConditions)
                {
                    refValues = _unitOfWork.RefValues.GetRefValuesWithConditionsByPublParamIdAndSample(
                        publParam.PublParamId, evalArgs.Sample);
                }
                else if (publication.UsesMediumSubTypes && publication.UsesConditions)
                {
                    refValues = _unitOfWork.RefValues.GetRefValuesWithMedSubTypesAndConditionsByPublParamIdAndSample(
                        publParam.PublParamId, evalArgs.Sample);
                }
                else
                { 
                    refValues = _unitOfWork.RefValues.GetRefValuesWithoutAttributesByPublParamId(publParam.PublParamId);
                }

                foreach (RefValue refValue in refValues)
                {
                    var exceedingValue = GetExceedingValue(evalArgs, publParam, refValue, labReportParams);
                    if (exceedingValue != null)
                    {
                        exceedingValues.Add(exceedingValue);

                        if (exceedingValue.Level > highestLevel)
                        {
                            highestLevel = exceedingValue.Level;
                        }
                    }
                }
            }
            var valClassStr = _unitOfWork.ValuationClasses.getValClassNameNextLevelFromLevel(
                highestLevel, publication.PublicationId);
            var valClassStrOneDown = _unitOfWork.ValuationClasses.getValClassNameNextLevelFromLevel(
                highestLevel - 1, publication.PublicationId);
            var highestValClassName = valClassStr.Length > 0 ? valClassStr : ">" + valClassStrOneDown;
            var exceedingValueList = "";

            foreach (ExceedingValue exceedingValue in exceedingValues)
            {
                if (exceedingValue.Level == highestLevel)
                {
                    var exceedingValuesStr = exceedingValue.ParamName +
                        " (" + exceedingValue.Value + " " + exceedingValue.Unit + ")";

                    if (exceedingValueList.Length == 0)
                    {
                        exceedingValueList += exceedingValuesStr;
                    }
                    else
                    {
                        exceedingValueList += Environment.NewLine + exceedingValuesStr;
                    }                  
                }
            }

            var missingParamsList = "";
            foreach (PublParam missingParam in _missingParams)
            {
                var missingParamName = _unitOfWork.Parameters.GetById(missingParam.ParameterId).ParamNameDe;
                var missingParamUnitName = _unitOfWork.Units.GetById(missingParam.UnitId).UnitName;
                missingParamsList += $"{missingParamName} ({missingParamUnitName}); ";
            }
            var minValueParamsList = "";
            foreach (string paramNameForMin in _paramNamesForMin)
            {
                minValueParamsList += $"{paramNameForMin}; ";
            }
            _evalResult.SampleName = evalArgs.Sample.SampleName;
            _evalResult.HighestValClassName = highestValClassName;
            _evalResult.ExceedingValues = exceedingValueList;
            _evalResult.MissingParams = missingParamsList;
            _evalResult.MinValueParams = minValueParamsList;
            return _evalResult;
        }

        private ExceedingValue GetExceedingValue(
            EvalArgs evalArgs, PublParam publParam, RefValue refValue, IEnumerable<LabReportParam> labReportParams)
        {
            var refVal = refValue.RValue;
            var refValUnitName = _unitOfWork.Units.GetById(publParam.UnitId).UnitName;
            var refValParam = _unitOfWork.Parameters.GetById(publParam.ParameterId);
            var refValParamNameDe = refValParam.ParamNameDe;
            var refValParamAnnot = refValParam.ParamAnnotation;
            var refValueValClass = _unitOfWork.ValuationClasses.GetById(refValue.ValuationClassId);
            var refValueValClassLevel = refValueValClass.ValClassLevel;

            var sampleId = evalArgs.Sample.SampleId;

            List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs = new();

            foreach (LabReportParam lrparam in labReportParams)
            {
                var sampleValueUnitName = _unitOfWork.Units.GetById(lrparam.UnitId).UnitName;
                var sValuesFromLrParam = _unitOfWork.SampleValues.GetSampleValuesBySampleIdAndLabReportParamId(
                                            sampleId, lrparam.LabReportParamId);

                foreach (SampleValue sValueFromLrParam in sValuesFromLrParam)
                {
                    var sValueConverted = _evalCalc.SampleValueConversion(sValueFromLrParam.SValue, sampleValueUnitName, refValUnitName);
                    LrParamSValuePairs.Add(new KeyValuePair<LabReportParam, double>(lrparam, sValueConverted));
                }
            }

            if (LrParamSValuePairs.Count() == 0)
            {
                _missingParams.Add(publParam);
                return null;
            }

            FinalSValue finalSValue = _evalCalc.GetFinalSValue(evalArgs, refValParamAnnot, LrParamSValuePairs);

            if (finalSValue.LabReportParamName.Length > 0)
            {
                _paramNamesForMin.Add(finalSValue.LabReportParamName);
            }

            if (_evalCalc.IsSampleValueExceedingRefValue(finalSValue.SValue, refVal, refValParamAnnot))
            {
                return new ExceedingValue()
                {
                    Level = refValueValClassLevel,
                    ParamName = refValParamNameDe,
                    Value = finalSValue.SValue,
                    Unit = refValUnitName
                };
            }
            return null;
        }
    }
}
