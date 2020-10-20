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
        /* TO DO: Refactor this spike. */
        
        private IUnitOfWork _unitOfWork;
        private EvalResult _evalResult;

        public EvalLabReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Sample Sample { get; private set; }

        public EvalResult getEvalResult(Guid sampleId, Guid publicationId)
        {
            _evalResult = new EvalResult();
            var sample = _unitOfWork.Samples.GetById(sampleId);
            var publication = _unitOfWork.Publications.GetById(publicationId);
            var highestLevel = 0;
            IEnumerable<RefValue> refValues = _unitOfWork.RefValues.GetRefValuesByPublicationId(publicationId);
            List<ExceedingValue> exceedingValues = new List<ExceedingValue>();

            foreach (var refValue in refValues)
            {
                var sampleValues = _unitOfWork.SampleValues.GetSampleValuesBySampleIdAndRefValue(sampleId, refValue);              
                var sampleVal = sampleValues.First().SValue;
                var sampleValUnitName = _unitOfWork.Units.GetById(sampleValues.First().UnitId).UnitName;

                var refVal = refValue.RValue;
                var refValUnitName = _unitOfWork.Units.GetById(refValue.UnitId).UnitName;
                var refValParam = _unitOfWork.Parameters.GetById(refValue.ParameterId);
                var refValParamNameDe = refValParam.ParamNameDe;
                var refValParamAnnot = refValParam.ParamAnnotation;

                var refValueValClass = _unitOfWork.ValuationClasses.GetById(refValue.ValuationClassId);
                var refValueValClassLevel = refValueValClass.ValClassLevel;
                var refValueValClassName = refValueValClass.ValuationClassName;

                if (refValUnitName.Length > 0 && refValUnitName.Substring(0, 1) == 
                    "m" && sampleValUnitName.Substring(0, 1) == "µ")
                    sampleVal /= 1000;
                else if (refValUnitName.Length > 0 && refValUnitName.Substring(0, 1) == 
                    "µ" && sampleValUnitName.Substring(0, 1) == "m")
                    sampleVal *= 1000;

                if (refValParamAnnot != "lower" && sampleVal > refVal 
                    || refValParamAnnot == "lower" && sampleVal < refVal)
                {
                    exceedingValues.Add(new ExceedingValue()
                    {
                        Level = refValueValClassLevel,
                        ParamName = refValParamNameDe,
                        Value = sampleVal,
                        Unit = sampleValUnitName
                    });
                    if (refValueValClassLevel > highestLevel)
                    { 
                        highestLevel = refValueValClassLevel;
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
    }
}
