using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.Model.Core
{
    /*
     * This class contains methods to evaluate footnotes of guidelines.
     * This is experimental and uncomplete.
     */
    public class Footnotes : IFootnotes
    {
        private IUnitOfWork _unitOfWork;
        private IEvalCalc _evalCalc;

        public Footnotes(IUnitOfWork unitOfWork, IEvalCalc evalCalc)
        {
            _unitOfWork = unitOfWork;
            _evalCalc = evalCalc;
        }

        public FootnoteResult IsFootnoteCondTrue(EvalArgs evalArgs, Guid? footnoteId)
        {
            var footnote = _unitOfWork.Footnotes.GetById(footnoteId);
            var footnoteParams = _unitOfWork.FootnoteParams.GetFootnoteParamsByFootnoteId(footnoteId);

            FootnoteResult footnoteResult = new();

            footnoteResult.Result = false;
            footnoteResult.IsNotExclusionCriterion = footnote.IsNotExclusionCriterion;
            HashSet<PublParam> missingParams = new();
            HashSet<string> takingAccountOf = new();
            
            footnoteResult.TakingAccountOf = takingAccountOf;
            footnoteResult.GeneralFootnoteTexts = footnote.GeneralFootnoteTexts;

            ExpressionContext context = new ExpressionContext();
            VariableCollection parameters = context.Variables;

            // Check condition in LabReportPreCheck: For matters of simplicity, this is only evaluated if the footnote
            // only has one expression to be evalauted, and if only one parameter is used in the expression.
            // In any other case, "true" is returned.
            if (evalArgs.Sample == null)
            {
                if (footnoteParams.Count() == 1 && footnote.Expression1.Length > 0 && footnote.Expression2.Length == 0)
                {
                    var publParam = _unitOfWork.PublParams.GetById(footnoteParams.FirstOrDefault().PublParamId);

                    var parameter = _unitOfWork.Parameters.GetById(publParam.ParameterId);
                    var unit = _unitOfWork.Units.GetById(publParam.UnitId);

                    var sampleValuesAndLrUnitNames = _unitOfWork.SampleValues.GetSampleValuesAndLrUnitNamesByLabReportIdParameterIdAndUnitName(
                    evalArgs.LabReportId, parameter.ParameterId, unit.UnitName);

                    if (sampleValuesAndLrUnitNames.Count() > 0)
                    { 
                        foreach (SampleValueAndLrUnitName svun in sampleValuesAndLrUnitNames)
                        {
                            var sValueConverted = _evalCalc.SampleValueConversion(
                                svun.sampleValue.SValue, svun.unitName, unit.UnitName);
                            parameters.Add($"param1", sValueConverted);
                            if (footnote.Expression1.Length > 0 && missingParams.Count == 0)
                            {
                                IGenericExpression<bool> e1 = context.CompileGeneric<bool>(footnote.Expression1);
                                var result = e1.Evaluate();
                                parameters.Clear();
                                if (result)
                                { 
                                    footnoteResult.Result = result;
                                    break;
                                }
                            }
                        }
                    }
                    return footnoteResult;
                }
                footnoteResult.Result = true;
                return footnoteResult;
            }
            else
            // Evaluate footnote in EvalLabReportService
            {
                int index = 1;

                foreach (FootnoteParam footnoteParam in footnoteParams)
                {
                    var publParam = _unitOfWork.PublParams.GetById(footnoteParam.PublParamId);

                    var parameter = _unitOfWork.Parameters.GetById(publParam.ParameterId);
                    var unit = _unitOfWork.Units.GetById(publParam.UnitId);

                    var finalSValue = GetFinalSValue(evalArgs, parameter.ParamAnnotation, unit.UnitName, publParam);
                    

                    if (finalSValue == -1)
                    {
                        missingParams.Add(publParam);
                    }
                    else
                    {
                        parameters.Add($"param{index}", finalSValue);
                    }

                    index++;               
                }

                bool result1 = false;
                bool result2;
                if (footnote.Expression1.Length > 0 && missingParams.Count == 0)
                {
                    IGenericExpression<bool> e1 = context.CompileGeneric<bool>(footnote.Expression1);
                    result1 = e1.Evaluate();
                    footnoteResult.Result = result1;
                }
                if (footnote.Expression2.Length > 0 && missingParams.Count == 0)
                {
                    IGenericExpression<bool> e2 = context.CompileGeneric<bool>(footnote.Expression2);
                    result2 = e2.Evaluate();
                    footnoteResult.Result = result1 && result2;
                }

                // TO DO: trying to find a solution to appropriately display missing params footnotes

                if (footnoteResult.Result)
                {
                    footnoteResult.TakingAccountOf.Add(footnote.FootnoteRef);
                }

                return footnoteResult;
            }
        }

        private double GetFinalSValue(EvalArgs evalArgs, string paramAnnotation, string unitName, PublParam publParam)
        {
            var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam, evalArgs.LabReportId);

            double sValue = -1;

            if (labReportParams.Count() != 0)
            {
                List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs =
                    _evalCalc.GetLrParamSValuePairs(labReportParams, evalArgs.Sample.SampleId, unitName);

                if (LrParamSValuePairs.Count() == 1)
                {
                    sValue = LrParamSValuePairs.First().Value;

                }
                else if (LrParamSValuePairs.Count() > 1)
                {
                    sValue = _evalCalc.GetFinalSValue(evalArgs, paramAnnotation, LrParamSValuePairs).SValue;
                }
            }

            return sValue;
        }
    }
}
