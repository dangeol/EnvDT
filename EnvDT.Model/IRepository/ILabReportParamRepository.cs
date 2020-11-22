using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ILabReportParamRepository : IGenericRepository<LabReportParam>
    {
        public IEnumerable<LabReportParam> GetLabReportParamNamesByPublParam(PublParam publParam, Guid labReportId);
        public IEnumerable<LabReportParam> GetLabReportParamsByPublParam(PublParam publParam, Guid labReportId);
        public IEnumerable<LabReportParam> GetLabReportUnknownParamNamesByLabReportId(Guid labReportId);
        public IEnumerable<LabReportParam> GetLabReportUnknownUnitNamesByLabReportId(Guid labReportId);
    }
}