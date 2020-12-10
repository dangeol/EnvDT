using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ILabReportParamRepository : IGenericRepository<LabReportParam>
    {
        public IEnumerable<LabReportParam> GetLabReportParamsByLabReportIdAndParamName(Guid labReportId, string labReportParamName);
        public IEnumerable<LabReportParam> GetLabReportParamsByByLabReportIdAndUnitName(Guid labReportId, string labReportUnitName);
        public IEnumerable<LabReportParam> GetLabReportParamNamesByPublParam(PublParam publParam, Guid labReportId);
        public IEnumerable<LabReportParam> GetLabReportParamsByPublParam(PublParam publParam, Guid labReportId);
    }
}