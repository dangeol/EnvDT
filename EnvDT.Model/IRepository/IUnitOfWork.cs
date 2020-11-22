﻿using System;

namespace EnvDT.Model.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        public ILabReportRepository LabReports { get; }
        public ILabReportParamRepository LabReportParams { get; }
        public ILanguageRepository Languages { get; }
        public IParameterRepository Parameters { get; }
        public IParamNameVariantRepository ParamNameVariants { get; }
        public IProjectRepository Projects { get; }
        public IPublicationRepository Publications { get; }
        public IPublParamRepository PublParams { get; }
        public IRefValueRepository RefValues { get; }
        public ISampleRepository Samples { get; }
        public ISampleValueRepository SampleValues { get; }
        public IUnitRepository Units { get; }
        public IUnitNameVariantRepository UnitNameVariants { get; }
        public IValuationClassRepository ValuationClasses { get; }
        public int Save();
    }
}
