using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.Model.Core
{
    /*
     * This class contains methods to evaluate footnotes of guidelines, grouped by country and guideline.
     */
    public class Footnotes : IFootnotes
    {
        private IUnitOfWork _unitOfWork;
        private IEvalCalc _evalCalc;
        private Dictionary<string, Func<EvalArgs, int, FootnoteResult>> _footnotes = new();

        public Footnotes(IUnitOfWork unitOfWork, IEvalCalc evalCalc)
        {
            _unitOfWork = unitOfWork;
            _evalCalc = evalCalc;
            InitializeFootnotes();
        }

        private void InitializeFootnotes()
        {
            /*
             * Format of the Keys:
             * [Abbreviation of Publication]_[Footnote reference]_[c/e]
             * [Footnote reference] is composend of [char][footnoteNumber]
             * with [char] either 'e' (eluate) or 's' (solid matter)
             */
             // GERMANY
             _footnotes.Add("Verf-Lf (BY)_e5", new Func<EvalArgs, int, FootnoteResult>(VerfLf_e5));
        }

        // with evalType 0: check in LabReportPreCheck if condition is met
        // with evalType 1: evaluation in EvalLabReportService which RefValue to take 
        public FootnoteResult IsFootnoteCondTrue(EvalArgs evalArgs, int evalType, string footnoteRef)
        {
            FootnoteResult footnoteResult = new();
            Func<EvalArgs, int, FootnoteResult> method;
            if (!_footnotes.TryGetValue(footnoteRef, out method))
            {
                footnoteResult.Result = true;
                return footnoteResult;
            }
            return method.Invoke(evalArgs, evalType);
        }

        private FootnoteSValueList GetFinalSValues(EvalArgs evalArgs, List<FootnoteParam> footnoteParams)
        {
            Dictionary<string, double> finalSValues = new();
            HashSet<PublParam> missingParams = new();           

            foreach (FootnoteParam footnoteParam in footnoteParams)
            {
                var publParam = _unitOfWork.PublParams.GetByPublIdParameterNameDeAndUnitName(
                    evalArgs.PublicationId, footnoteParam.FootnoteParamName, footnoteParam.FootnoteUnitName);
                var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam, evalArgs.LabReportId);

                if (labReportParams.Count() == 0)
                {
                    finalSValues.Add(footnoteParam.FootnoteParamName, 0.0);
                    missingParams.Add(publParam);
                    break;
                }

                List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs =
                    _evalCalc.GetLrParamSValuePairs(labReportParams, evalArgs.Sample.SampleId, footnoteParam.FootnoteUnitName);

                double sValue = 0;

                if (LrParamSValuePairs.Count() == 1)
                {
                    sValue = LrParamSValuePairs.First().Value;
                }
                else if (LrParamSValuePairs.Count() > 1)
                {
                    sValue = _evalCalc.GetFinalSValue(evalArgs, footnoteParam.FootnoteParamAnnot, LrParamSValuePairs).SValue;
                }

                finalSValues.Add(footnoteParam.FootnoteParamName, sValue);
            }

            FootnoteSValueList footnoteSValues = new()
            {
                FinalSValues = finalSValues,
                MissingParams = missingParams
            };
            return footnoteSValues;
        }

        /*
         * ========================================================================================
         * GERMANY
         * ========================================================================================
         */

        /*
         * Verf-Lf (BY) Footnote Eluate 5:
         * "Bei Überschreitung des Z 1.1-Werts für Chrom(gesamt) von 30 μg/l ist der Anteil an Cr(VI) (Chromat) zu bestimmen.
         * Der Cr(VI)-Gehalt darf für eine Z 1.1-Einstufung 8 μg/l nicht überschreiten.
         * Diese Regel gilt bis zu einem maximalen Chrom(gesamt)-Wert von 50 μg/l.
         * Überschreitet das Material den Cr(VI)-Wert von 8 μg/l, ist das Material als Z 1.2 einzustufen.
         * Für Material der Klasse Z 1.2 und Z 2 ist eine Bewertung des Cr(VI)-Eluatwerts nicht vorgesehen und nicht einstufungsrelevant, 
         * es genügt die Bestimmung von Chrom(gesamt)."
         */
        private FootnoteResult VerfLf_e5(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new();
            footnoteResult.Result = false;

            List<FootnoteParam> footnoteParams = new()
            {
                new FootnoteParam() { FootnoteParamName = "Chrom Gesamt", FootnoteUnitName = "µg/l", FootnoteParamAnnot = "" },
                new FootnoteParam() { FootnoteParamName = "Chrom VI", FootnoteUnitName = "µg/l", FootnoteParamAnnot = "" },
            };
            switch (evalType)
            {
                case 0:
                    // Check condition:
                    // return true if (Chrom Gesamt > 30 µg / l && Chrom Gesamt <= 50 µg / l)
                    var sampleValuesAndLrUnitNames = _unitOfWork.SampleValues.GetSampleValuesAndLrUnitNamesByLabReportIdParamNameDeAndUnitName(
                        evalArgs.LabReportId, footnoteParams[0].FootnoteParamName, footnoteParams[0].FootnoteUnitName);

                    foreach (SampleValueAndLrUnitName svun in sampleValuesAndLrUnitNames)
                    {
                        var sValueConverted = _evalCalc.SampleValueConversion(
                            svun.sampleValue.SValue, svun.unitName, footnoteParams[0].FootnoteUnitName);
                        if (sValueConverted > 30 && sValueConverted <= 50)
                        {
                            footnoteResult.Result = true;
                            break;
                        }
                    }
                    return footnoteResult;

                case 1:
                    // Evaluate footnote:
                    // return true if Chrom Gesamt > 30 μg/l && Chrom Gesamt <= 50 μg/l && Cr (VI) <= 8 μg/l
                    FootnoteSValueList finalSValues = GetFinalSValues(evalArgs, footnoteParams);
                    var values = finalSValues.FinalSValues;
                    footnoteResult.MissingParams = finalSValues.MissingParams;

                    if (finalSValues.MissingParams.Count == 0 && 
                        values["Chrom Gesamt"] > 30 && values["Chrom Gesamt"] <= 50 && values["Cr (VI)"] <= 8)
                    {
                        footnoteResult.Result = true;
                    }

                    return footnoteResult;

                default:
                    return footnoteResult;
            }
        }       
    }
}
