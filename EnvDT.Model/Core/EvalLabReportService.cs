﻿using EnvDT.Model.Entity;
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

        public EvalLabReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Sample Sample { get; private set; }

        public void evalLabReport(Guid sampleId, Guid publicationId)
        {
            compareValues(sampleId, publicationId);
        }

        private void compareValues(Guid sampleId, Guid publicationId)
        {
            var sample = _unitOfWork.Samples.GetById(sampleId);
            var publication = _unitOfWork.Publications.GetById(publicationId);
            System.Diagnostics.Debug.WriteLine(sample.SampleName);
            System.Diagnostics.Debug.WriteLine("---------------------------------");

            var refValues = _unitOfWork.RefValues.GetRefValuesByPublicationId(publicationId);

            var highestLevel = 0;

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

                if (refValUnitName.Length > 0 && refValUnitName.Substring(0, 1) == "m" && sampleValUnitName.Substring(0, 1) == "µ")
                    sampleVal /= 1000;
                else if (refValUnitName.Length > 0 && refValUnitName.Substring(0, 1) == "µ" && sampleValUnitName.Substring(0, 1) == "m")
                    sampleVal *= 1000;

                if (refValParamAnnot != "lower")
                {
                    if (sampleVal > refVal)
                    {
                        if (refValueValClassLevel > highestLevel)
                            highestLevel = refValueValClassLevel;

                        exceedingValues.Add(new ExceedingValue()
                        {
                            Level = refValueValClassLevel,
                            ParamName = refValParamNameDe,
                            Value = sampleVal,
                            Unit = sampleValUnitName
                        });
                    }
                } 
                else
                {
                    if (sampleVal < refVal)
                    {
                        if (refValueValClassLevel > highestLevel)
                            highestLevel = refValueValClassLevel;

                        exceedingValues.Add(new ExceedingValue()
                        {
                            Level = refValueValClassLevel,
                            ParamName = refValParamNameDe,
                            Value = sampleVal,
                            Unit = sampleValUnitName
                        });
                    }
                }
            }
            var valClassStr = _unitOfWork.ValuationClasses.getValClassNameNextLevelFromLevel(highestLevel, publication.PublicationId);
            var highestValClassName = valClassStr.Length > 0 ? valClassStr : ">" + valClassStr;

            System.Diagnostics.Debug.WriteLine("+++++ Einstufung: " + highestValClassName);
            foreach (ExceedingValue exceedingValue in exceedingValues)
            {
                if (exceedingValue.Level == highestLevel)
                {
                    System.Diagnostics.Debug.WriteLine(exceedingValue.ParamName + " (" + exceedingValue.Value + " " + exceedingValue.Unit + ")");
                }
            }
        }
    }
}
