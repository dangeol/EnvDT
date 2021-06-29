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
        private IFootnotes _footnotes;
        private EvalResult _evalResult;
        private Publication _publication;
        private HashSet<string> _generalFootnoteTexts = new();
        private HashSet<PublParam> _missingParams = new();
        private HashSet<string> _paramNamesForMin = new();
        private HashSet<string> _toleranceParams = new();
        private HashSet<string> _takingAccountOf = new();

        public EvalLabReportService(IUnitOfWork unitOfWork, ILabReportPreCheck labReportPreCheck, 
            IEvalCalc evalCalc, IFootnotes footnotes)
        {
            _unitOfWork = unitOfWork;
            _labReportPreCheck = labReportPreCheck;
            _evalCalc = evalCalc;
            _footnotes = footnotes;
        }

        public bool LabReportPreCheck(Guid labReportId, IReadOnlyCollection<Guid> publicationIds)
        {
            // TO DO: refactor - find synergies with GetEvalResult method to increase efficiency
            return _labReportPreCheck.FindMissingParametersUnits(labReportId, publicationIds);
        }

        public EvalResult GetEvalResult(EvalArgs evalArgs)
        {
            _generalFootnoteTexts.Clear();
            _missingParams.Clear();
            _paramNamesForMin.Clear();
            _toleranceParams.Clear();
            _evalResult = new EvalResult();
            _publication = _unitOfWork.Publications.GetById(evalArgs.PublicationId);
            var publParams = _publication.PublParams;
            var highestLevel = 0;
            List<ExceedingValue> exceedingValues = new();

            foreach (PublParam publParam in publParams)
            {
                var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam, evalArgs.LabReportId);

                if (labReportParams.Count() == 0)
                {
                    if (publParam.IsMandatory && publParam.FootnoteId == null)
                    {
                        _missingParams.Add(publParam);
                    }                    
                    continue;
                }

                var refValues = Enumerable.Empty<RefValue>();

                if (_publication.UsesMediumSubTypes && !_publication.UsesConditions)
                {
                    refValues = _unitOfWork.RefValues.GetRefValuesWithMedSubTypesByPublParamIdAndSample(
                        publParam.PublParamId, evalArgs.Sample);
                }
                else if (!_publication.UsesMediumSubTypes && _publication.UsesConditions)
                {
                    refValues = _unitOfWork.RefValues.GetRefValuesWithConditionsByPublParamIdAndSample(
                        publParam.PublParamId, evalArgs.Sample);
                }
                else if (_publication.UsesMediumSubTypes && _publication.UsesConditions)
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
                    if (publParam.FootnoteId == null || 
                        (publParam.FootnoteId != null &&
                        _footnotes.IsFootnoteCondTrue(evalArgs, publParam.FootnoteId).Result))
                    { 
                        var exceedingValue = GetExceedingValue(evalArgs, publParam, refValue, labReportParams);
                        if (exceedingValue != null)
                        {
                            exceedingValues.Add(exceedingValue);

                            if (!exceedingValue.IsNotExclusionCriterion && exceedingValue.Level > highestLevel)
                            {
                                highestLevel = exceedingValue.Level;
                            }
                        }
                    }
                }
            }
            var valClassStr = _unitOfWork.ValuationClasses.GetValClassNameNextLevelFromLevel(
                highestLevel, _publication.PublicationId);
            var valClassStrOneDown = _unitOfWork.ValuationClasses.GetValClassNameNextLevelFromLevel(
                highestLevel - 1, _publication.PublicationId);
            var highestValClassName = valClassStr.Length > 0 ? valClassStr : ">" + valClassStrOneDown;
            var exceedingValueList = "";
            HashSet<string> testList = new();
            bool mustGeneralFootnoteTextsBeShown = false;

            foreach (ExceedingValue exceedingValue in exceedingValues)
            {              
                if (exceedingValue.Level >= highestLevel)
                {
                    var exceedingValuesStrOrig = $"{exceedingValue.ParamName} ({exceedingValue.Value} {exceedingValue.Unit})";
                    var exceedingValuesStr = exceedingValuesStrOrig;

                    if (!testList.Contains(exceedingValuesStr))
                    { 
                        if (exceedingValue.IsNotExclusionCriterion)
                        {
                            exceedingValuesStr += "\u2070\u207E";
                        }
                    
                        if (exceedingValueList.Length == 0)
                        {
                            exceedingValueList += exceedingValuesStr;
                        }
                        else
                        {
                            exceedingValueList += Environment.NewLine + exceedingValuesStr;
                        }

                        testList.Add(exceedingValuesStrOrig);
                    }
                }
                if (exceedingValue.Level > highestLevel)
                {
                    mustGeneralFootnoteTextsBeShown = true;
                }
            }

            if (!mustGeneralFootnoteTextsBeShown)
            {
                _generalFootnoteTexts.Clear();
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
            var toleranceParamsList = "";
            foreach (string toleranceParam in _toleranceParams)
            {
                toleranceParamsList += $"{toleranceParam}; ";
            }
            var takingAccountOfList = "";
            foreach (string takingAccountOf in _takingAccountOf)
            {
                takingAccountOfList += $"{takingAccountOf}; ";
            }
            // TO DO: This here is a Hack to account for specifities of German guidelines. Should be implemented programmatically.
            /*
            if (Guid.Equals(evalArgs.Sample.MediumSubTypeId, new Guid("65dd3830-ec8f-4be2-b47e-92926e964f50"))
                && highestValClassName.Equals("Z0"))
            {
                highestValClassName = "Z1.1";
                if (!exceedingValueList.Contains("\u207E"))
                {
                    exceedingValueList = "";
                }
            }*/
            // end of Hack.
            _evalResult.SampleName = evalArgs.Sample.SampleName;
            _evalResult.HighestValClassName = highestValClassName;
            _evalResult.ExceedingValues = exceedingValueList;
            _evalResult.GeneralFootnoteTexts = _generalFootnoteTexts;
            _evalResult.MissingParams = missingParamsList;
            _evalResult.MinValueParams = minValueParamsList;
            _evalResult.ToleranceParams = toleranceParamsList;
            _evalResult.TakingAccountOf = takingAccountOfList;
            return _evalResult;
        }

        private ExceedingValue GetExceedingValue(
            EvalArgs evalArgs, PublParam publParam, RefValue refValue, IEnumerable<LabReportParam> labReportParams)
        {
            var sampleId = evalArgs.Sample.SampleId;
            bool isNotExclusionCriterion = false;
            var generalFootnoteTexts = "";

            double refVal;
            if ((publParam.FootnoteId != null || refValue.FootnoteId != null) && evalArgs.EvalFootnotes)
            {
                var footnoteId = refValue.FootnoteId != null ? refValue.FootnoteId : publParam.FootnoteId;

                FootnoteResult footnoteResult = _footnotes.IsFootnoteCondTrue(evalArgs, footnoteId);
                isNotExclusionCriterion = footnoteResult.IsNotExclusionCriterion;
                bool shouldRefValueAltBeTaken = footnoteResult.Result;
                refVal = refValue.RValueAlt > 0 && shouldRefValueAltBeTaken ? refValue.RValueAlt : refValue.RValue;

                if (footnoteResult.GeneralFootnoteTexts != null)
                {
                    generalFootnoteTexts = footnoteResult.GeneralFootnoteTexts;
                }
                if (footnoteResult.MissingParams != null)
                { 
                    _missingParams.UnionWith(footnoteResult.MissingParams);
                }
                if (footnoteResult.TakingAccountOf != null)
                {
                    _takingAccountOf.UnionWith(footnoteResult.TakingAccountOf);
                }
            }
            else
            {
                refVal = refValue.RValue;
            }
            var refValUnitName = _unitOfWork.Units.GetById(publParam.UnitId).UnitName;
            var refValParam = _unitOfWork.Parameters.GetById(publParam.ParameterId);
            var refValParamNameDe = refValParam.ParamNameDe;
            var refValParamAnnot = refValParam.ParamAnnotation;
            var refValueValClass = _unitOfWork.ValuationClasses.GetById(refValue.ValuationClassId);
            var refValueValClassLevel = refValueValClass.ValClassLevel;

            List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs = 
                _evalCalc.GetLrParamSValuePairs(labReportParams, sampleId, refValUnitName);

            FinalSValue finalSValue = new();

            if (LrParamSValuePairs.Count() == 0)
            {
                _missingParams.Add(publParam);
                return null;
            }
            else if (LrParamSValuePairs.Count() == 1)
            {
                finalSValue.SValue = LrParamSValuePairs.First().Value;
            }
            else
            {
                finalSValue = _evalCalc.GetFinalSValue(evalArgs, refValParamAnnot, LrParamSValuePairs);
                if (finalSValue.LabReportParamName.Length > 0)
                {
                    _paramNamesForMin.Add(finalSValue.LabReportParamName);
                }
            }
            var refValUnchanged = refVal;
            if (publParam.Tolerance > 0)
            {              
                refVal += refVal * publParam.Tolerance;
                if (finalSValue.SValue > refValUnchanged && finalSValue.SValue <= refVal)
                { 
                    _toleranceParams.Add($"{refValParamNameDe} ({refValUnitName})");
                }
            }

            if (_evalCalc.IsSampleValueExceedingRefValue(finalSValue.SValue, refVal, refValParamAnnot))
            {
                _generalFootnoteTexts.Add(generalFootnoteTexts);

                return new ExceedingValue()
                {
                    Level = refValueValClassLevel,
                    ParamName = refValParamNameDe,
                    Value = finalSValue.SValue,
                    Unit = refValUnitName,
                    IsNotExclusionCriterion = isNotExclusionCriterion
                };
            }
            return null;
        }
    }
}
