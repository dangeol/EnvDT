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
        private IEvalCalcService _evalCalcService;
        private EvalResult _evalResult;

        public EvalLabReportService(IUnitOfWork unitOfWork, IEvalCalcService evalCalcService)
        {
            _unitOfWork = unitOfWork;
            _evalCalcService = evalCalcService;
        }

        public EvalResult getEvalResult(Guid sampleId, Guid publicationId)
        {
            _evalResult = new EvalResult();
            var sample = _unitOfWork.Samples.GetById(sampleId);
            var publication = _unitOfWork.Publications.GetById(publicationId);
            var publParams = publication.PublParams;
            var highestLevel = 0;
            List<ExceedingValue> exceedingValues = new List<ExceedingValue>();

            foreach (PublParam publParam in publParams)
            {
                var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam);
                var labReportParam = labReportParams.First();
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
            _evalResult.SampleName = sample.SampleName;
            _evalResult.HighestValClassName = highestValClassName;
            _evalResult.ExceedingValueList = exceedingValueList;
            return _evalResult;
        }

        private ExceedingValue GetExceedingValue(Guid sampleId, PublParam publParam, RefValue refValue, LabReportParam labReportParam)
        {
            var sampleValues = _unitOfWork.SampleValues.GetSampleValuesBySampleIdAndLabReportParam(sampleId, labReportParam.LabReportParamId);
            // Next line: treat null
            var sampleValue = sampleValues.First().SValue;
            var sampleValueUnitName = _unitOfWork.Units.GetById(labReportParam.UnitId).UnitName;

            var refVal = refValue.RValue;
            var refValUnitName = _unitOfWork.Units.GetById(publParam.UnitId).UnitName;
            var refValParam = _unitOfWork.Parameters.GetById(publParam.ParameterId);
            var refValParamNameDe = refValParam.ParamNameDe;
            var refValParamAnnot = refValParam.ParamAnnotation;
            var refValueValClass = _unitOfWork.ValuationClasses.GetById(refValue.ValuationClassId);
            var refValueValClassLevel = refValueValClass.ValClassLevel;

            sampleValue = _evalCalcService.SampleValueConversion(sampleValue, sampleValueUnitName, refValUnitName);

            if (_evalCalcService.IsSampleValueExceedingRefValue(sampleValue, refVal, refValParamAnnot))
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
