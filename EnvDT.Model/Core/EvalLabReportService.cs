using EnvDT.Model.Core;
using EnvDT.Model.Entity;
using EnvDT.Model.Core.HelperClasses;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using EnvDT.Model.IRepository;

namespace EnvDT.Model.Core
{
    public class EvalLabReportService : IEvalLabReportService
    {
        /* TO DO: Refactor this spike. 
        
        private IUnitOfWork _unitOfWork;

        private const string projectName = "Sample-Project 1";
        private const string publicationAbbr = "Dihlmann-Erlass";

        public EvalLabReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void evalLabReport()
    {
            var project = _unitOfWork.Projects
                .Single(p => p.ProjectName == projectName);

            var labReport = ctx.LabReports
                .First(l => l.Project.Equals(project));

            var samples = from sample in ctx.Samples
                            where sample.LabReport.Equals(labReport)
                            select sample;

            foreach (var sample in samples)
            {
                compareValues(sample);
            }
        }

        private void compareValues(Sample sample)
        {
            System.Diagnostics.Debug.WriteLine(sample.SampleName);
            System.Diagnostics.Debug.WriteLine("---------------------------------");

            using (var ctx = _contextCreator())
            {
                var publication = ctx.Publications
                    .Single(p => p.Abbreviation == publicationAbbr);

                var refValues = from refValue in ctx.RefValues
                                where refValue.Publication.Equals(publication)
                                select refValue;

                var highestLevel = 0;

                List<ExceedingValue> exceedingValues = new List<ExceedingValue>();

                foreach (var refValue in refValues)
                {
                    var sampleValueQuery =
                        from s in ctx.SampleValues
                            .Where(s => s.SampleId == sample.SampleId && s.ParameterId == refValue.ParameterId)
                        join su in ctx.Units on s.UnitId equals su.UnitId
                        join ru in ctx.Units on refValue.UnitId equals ru.UnitId
                        where (su.UnitName.Length > 0 &&
                            ru.UnitName.Length > 0 &&
                            su.UnitName.Substring(1, su.UnitName.Length - 1).Equals(ru.UnitName.Substring(1, ru.UnitName.Length - 1))) ||
                            (su.UnitName.Length ==  0 && ru.UnitName.Length == 0)
                        select s;

                    var refVal = refValue.RValue;
                    var sampleVal = sampleValueQuery.First().SValue;

                    var refValUnitName = ctx.Units
                        .First(u => u.UnitId == refValue.UnitId).UnitName;

                    var refValParamName = ctx.Parameters
                        .First(p => p.ParameterId == refValue.ParameterId).ParamNameDe;

                    var refValParamAnnot = ctx.Parameters
                        .First(p => p.ParameterId == refValue.ParameterId).ParamAnnotation;

                    var sampleValUnitName = ctx.Units
                        .First(u => u.UnitId == sampleValueQuery.First().UnitId).UnitName;

                    var refValueParamNameDe = ctx.Parameters
                        .First(p => p.ParameterId == refValue.ParameterId).ParamNameDe;

                    var refValueValClassLevel = ctx.ValuationClasses
                        .First(v => v.ValuationClassId == refValue.ValuationClassId).ValClassLevel;

                    var refValueValClassName = ctx.ValuationClasses
                        .First(v => v.ValuationClassId == refValue.ValuationClassId).ValuationClassName;

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
                                ParamName = refValueParamNameDe,
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
                                ParamName = refValueParamNameDe,
                                Value = sampleVal,
                                Unit = sampleValUnitName
                            });
                        }
                    }
                }
                var valClassStr = getValClassNameNextLevelFromLevel(highestLevel, publication);
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

        private string getValClassNameNextLevelFromLevel(int level, Publication publication)
        {
            using (var ctx = _contextCreator())
            {
                return ctx.ValuationClasses
                .FirstOrDefault(v => v.ValClassLevel == level + 1 && v.PublicationId == publication.PublicationId)?
                    .ValuationClassName ?? string.Empty;
            }
        }*/
    }
}
