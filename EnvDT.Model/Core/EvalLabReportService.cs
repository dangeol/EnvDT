using EnvDT.Model.Entity;
using EnvDT.Model.Core.HelperClasses;
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
        List<PublParam> _missingParams = new List<PublParam>();

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

        public EvalResult GetEvalResult(Guid labReportId, Guid sampleId, Guid publicationId)
        {
            _evalResult = new EvalResult();
            var sample = _unitOfWork.Samples.GetById(sampleId);
            var publication = _unitOfWork.Publications.GetById(publicationId);
            var publParams = publication.PublParams;
            var highestLevel = 0;
            List<ExceedingValue> exceedingValues = new List<ExceedingValue>();

            foreach (PublParam publParam in publParams)
            {
                var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam, labReportId);
                var labReportParam = new LabReportParam();

                if (labReportParams.Count() == 0)
                {
                    _missingParams.Add(publParam);
                    continue;
                }
                else
                {
                    labReportParam = labReportParams.First();
                }
                
                var refValues = _unitOfWork.RefValues.GetRefValuesByPublParamId(publParam.PublParamId);
                foreach (RefValue refValue in refValues)
                {
                    var exceedingValue = GetExceedingValue(sampleId, publParam, refValue, labReportParam);
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
            var highestValClassName = valClassStr.Length > 0 ? valClassStr : ">" + valClassStr;
            var exceedingValueList = "";

            foreach (ExceedingValue exceedingValue in exceedingValues)
            {
                if (exceedingValue.Level == highestLevel)
                {
                    exceedingValueList += exceedingValue.ParamName + 
                        " (" + exceedingValue.Value + " " + exceedingValue.Unit + ")";
                }
            }

            var missingParamsList = "";
            foreach (PublParam missingParam in _missingParams)
            {
                var missingParamName = _unitOfWork.Parameters.GetById(missingParam.ParameterId).ParamNameDe;
                var missingParamUnitName = _unitOfWork.Units.GetById(missingParam.UnitId).UnitName;
                missingParamsList += missingParamName + " (" + missingParamUnitName + ")";
            }
            _evalResult.SampleName = sample.SampleName;
            _evalResult.HighestValClassName = highestValClassName;
            _evalResult.ExceedingValues = exceedingValueList;
            _evalResult.MissingParams = missingParamsList;
            return _evalResult;
        }

        private ExceedingValue GetExceedingValue(Guid sampleId, PublParam publParam, RefValue refValue, LabReportParam labReportParam)
        {
            var sampleValues = _unitOfWork.SampleValues.GetSampleValuesBySampleIdAndLabReportParamId(sampleId, labReportParam.LabReportParamId);
            double sampleValue;
            if (sampleValues.Count() == 0)
            {
                _missingParams.Add(publParam);
                return null;
            }
            else
            {
                sampleValue = sampleValues.First().SValue;
            }
            var sampleValueUnitName = _unitOfWork.Units.GetById(labReportParam.UnitId).UnitName;

            var refVal = refValue.RValue;
            var refValUnitName = _unitOfWork.Units.GetById(publParam.UnitId).UnitName;
            var refValParam = _unitOfWork.Parameters.GetById(publParam.ParameterId);
            var refValParamNameDe = refValParam.ParamNameDe;
            var refValParamAnnot = refValParam.ParamAnnotation;
            var refValueValClass = _unitOfWork.ValuationClasses.GetById(refValue.ValuationClassId);
            var refValueValClassLevel = refValueValClass.ValClassLevel;

            sampleValue = _evalCalc.SampleValueConversion(sampleValue, sampleValueUnitName, refValUnitName);

            if (_evalCalc.IsSampleValueExceedingRefValue(sampleValue, refVal, refValParamAnnot))
            {
                return new ExceedingValue()
                {
                    Level = refValueValClassLevel,
                    ParamName = refValParamNameDe,
                    Value = sampleValue,
                    Unit = sampleValueUnitName
                };
            }
            return null;
        }
    }
}
