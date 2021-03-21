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
     * This is experimental and uncomplete.
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
             * [PublIdent of Publication]_[Footnote reference]_[c/e]
             * [Footnote reference] is composend of [char][footnoteNumber]
             * with [char] either 'e' (eluate) or 's' (solid matter)
             */
            // GERMANY
            _footnotes.Add("VerfLfBY_e1", new Func<EvalArgs, int, FootnoteResult>(VerfLfBY_e1));
            _footnotes.Add("VerfLfBY_e5", new Func<EvalArgs, int, FootnoteResult>(VerfLfBY_e5));
            _footnotes.Add("VwVBW_e1", new Func<EvalArgs, int, FootnoteResult>(VwVBW_e1));
            _footnotes.Add("DihlmEBW_e5", new Func<EvalArgs, int, FootnoteResult>(DihlmEBW_e5));
            _footnotes.Add("DepV_e8", new Func<EvalArgs, int, FootnoteResult>(DepV_e8));
            _footnotes.Add("DepVBY_e13", new Func<EvalArgs, int, FootnoteResult>(DepVBY_e13));
        }

        // with evalType 0: check in LabReportPreCheck if condition is met
        // with evalType 1: evaluation in EvalLabReportService which RefValue to take 
        public FootnoteResult IsFootnoteCondTrue(EvalArgs evalArgs, int evalType, string footnoteRef)
        {
            FootnoteResult footnoteResult = new();
            Func<EvalArgs, int, FootnoteResult> method;
            if (!_footnotes.TryGetValue(footnoteRef, out method))
            {
                footnoteResult.Result = false;
                return footnoteResult;
            }
            return method.Invoke(evalArgs, evalType);
        }

        private FootnoteSValueList GetFinalSValues(EvalArgs evalArgs, Dictionary<string, FootnoteParam> footnoteParams)
        {
            Dictionary<string, double> finalSValues = new();
            HashSet<PublParam> missingParams = new();

            foreach (FootnoteParam footnoteParam in footnoteParams.Values)
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
         * Verf-Lf (BY) Footnote Eluate 1:
         */
        private FootnoteResult VerfLfBY_e1(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new()
            {
                Result = false,
                IsNotExclusionCriterion = true,
                GeneralFootnoteTexts = "Abweichungen von den Bereichen der Zuordnungswerte für den pH-Wert und/oder " +
                "die Überschreitung der elektrischen Leitfähigkeit im Eluat stellen allein kein Ausschlusskriterium dar, " +
                "die Ursache ist im Einzelfall zu prüfen und zu dokumentieren."
            };

            return footnoteResult;
        }

        /*
         * Verf-Lf (BY) Footnote Eluate 5:
         * "Bei Überschreitung des Z 1.1-Werts für Chrom(gesamt) von 30 μg/l ist der Anteil an Cr(VI) (Chromat) zu bestimmen.
         * Der Cr(VI)-Gehalt darf für eine Z 1.1-Einstufung 8 μg/l nicht überschreiten.
         * Diese Regel gilt bis zu einem maximalen Chrom(gesamt)-Wert von 50 μg/l.
         * Überschreitet das Material den Cr(VI)-Wert von 8 μg/l, ist das Material als Z 1.2 einzustufen.
         * Für Material der Klasse Z 1.2 und Z 2 ist eine Bewertung des Cr(VI)-Eluatwerts nicht vorgesehen und nicht einstufungsrelevant, 
         * es genügt die Bestimmung von Chrom(gesamt)."
         */
        private FootnoteResult VerfLfBY_e5(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new();
            footnoteResult.Result = false;
            footnoteResult.IsNotExclusionCriterion = false;
            HashSet<PublParam> missingParams = new();
            HashSet<string> takingAccountOf = new();                                 
            footnoteResult.MissingParams = missingParams;
            footnoteResult.TakingAccountOf = takingAccountOf;

            Dictionary<string, FootnoteParam> footnoteParams = new()
            {
                { 
                    "Chrom Gesamt", 
                    new FootnoteParam() { FootnoteParamName = "Chrom Gesamt", FootnoteUnitName = "µg/l", FootnoteParamAnnot = "" } 
                },
                { 
                    "Chrom VI", 
                    new FootnoteParam() { FootnoteParamName = "Chrom VI", FootnoteUnitName = "µg/l", FootnoteParamAnnot = "" } 
                },
            };
            switch (evalType)
            {
                case 0:
                    // Check condition in LabReportPreCheck:
                    // return true if (Chrom Gesamt > 30 µg / l && Chrom Gesamt <= 50 µg / l)
                    var sampleValuesAndLrUnitNames = _unitOfWork.SampleValues.GetSampleValuesAndLrUnitNamesByLabReportIdParamNameDeAndUnitName(
                        evalArgs.LabReportId, footnoteParams["Chrom Gesamt"].FootnoteParamName, footnoteParams["Chrom Gesamt"].FootnoteUnitName);

                    foreach (SampleValueAndLrUnitName svun in sampleValuesAndLrUnitNames)
                    {
                        var sValueConverted = _evalCalc.SampleValueConversion(
                            svun.sampleValue.SValue, svun.unitName, footnoteParams["Chrom Gesamt"].FootnoteUnitName);
                        if (sValueConverted > 30 && sValueConverted <= 50)
                        {
                            footnoteResult.Result = true;
                            break;
                        }
                    }
                    return footnoteResult;

                case 1:
                    // Evaluate footnote in EvalLabReportService:
                    // return true if Chrom Gesamt > 30 μg/l && Chrom Gesamt <= 50 μg/l && Cr (VI) <= 8 μg/l
                    FootnoteSValueList finalSValues = GetFinalSValues(evalArgs, footnoteParams);
                    var values = finalSValues.FinalSValues;
                    
                    if (values["Chrom Gesamt"] > 30 && values["Chrom Gesamt"] <= 50)
                    {
                        footnoteResult.MissingParams = finalSValues.MissingParams;

                        if (finalSValues.MissingParams.Count == 0 && values["Chrom VI"] <= 8)
                        {
                            footnoteResult.Result = true;
                            footnoteResult.TakingAccountOf.Add("Fußnote 5 (Eluat)");
                        }
                    }
                    return footnoteResult;

                default:
                    return footnoteResult;
            }
        }

        /*
         * VwV (BW) Footnote Eluate 1:
         */
        private FootnoteResult VwVBW_e1(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new()
            {
                Result = false,
                IsNotExclusionCriterion = true,
                GeneralFootnoteTexts = "Eine Überschreitung dieser Parameter allein ist kein Ausschlusskriterium."
            };

            return footnoteResult;
        }

        /*
         * Dihlm-E (BW) Footnote Eluate 5:
         */
        private FootnoteResult DihlmEBW_e5(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new()
            {
                Result = false,
                IsNotExclusionCriterion = true,
                GeneralFootnoteTexts = "(pH-Wert): pH-Werte stellen allein kein Ausschlusskriterium dar."
            };

            return footnoteResult;
        }

        /*
         * DepV Footnote Eluate 8:
         */
        private FootnoteResult DepV_e8(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new()
            {
                Result = false,
                IsNotExclusionCriterion = true,
                GeneralFootnoteTexts = "Abweichende pH-Werte stellen allein kein Ausschlusskriterium dar. Bei Über- oder Unterschreitungen " +
                "ist die Ursache zu prüfen. Werden jedoch auf Deponien der Klassen I und II gefährliche Abfälle abgelagert, " +
                "muss deren pH - Wert mindestens 6,0 betragen."
            };

            return footnoteResult;
        }

        /*
         * DepV (BY) Footnote Eluate 13:
         */
        private FootnoteResult DepVBY_e13(EvalArgs evalArgs, int evalType)
        {
            FootnoteResult footnoteResult = new()
            {
                Result = false,
                IsNotExclusionCriterion = true,
                GeneralFootnoteTexts = "Abweichende pH-Werte stellen allein kein Ausschlusskriterium dar. Bei Über- oder Unterschreitungen " +
                "ist die Ursache zu prüfen. Werden jedoch auf Deponien der Klassen I und II gefährliche Abfälle abgelagert, " +
                "muss deren pH - Wert mindestens 6 betragen."
            };

            return footnoteResult;
        }
    }
}
