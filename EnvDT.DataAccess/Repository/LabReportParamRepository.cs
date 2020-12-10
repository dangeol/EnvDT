using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class LabReportParamRepository : GenericRepository<LabReportParam, EnvDTDbContext>,
		ILabReportParamRepository
	{
		public LabReportParamRepository(EnvDTDbContext context)
			: base(context)
		{
		}

		public IEnumerable<LabReportParam> GetLabReportParamsByLabReportIdAndParamName(Guid labReportId, string labReportParamName)
		{
			return
			(
				from lp in Context.LabReportParams
					.Where(lp => lp.LabReportId == labReportId && lp.LabReportParamName == labReportParamName)
				select lp
			)
			.ToList();
		}

		public IEnumerable<LabReportParam> GetLabReportParamsByByLabReportIdAndUnitName(Guid labReportId, string labReportUnitName)
		{
			return
			(
				from lp in Context.LabReportParams
					.Where(lp => lp.LabReportId == labReportId && lp.LabReportUnitName == labReportUnitName)
				select lp
			)
			.ToList();
		}

		public IEnumerable<LabReportParam> GetLabReportParamNamesByPublParam(PublParam publParam, Guid labReportId)
		{
			return
			(
				from lp in Context.LabReportParams
					.Where(lp => lp.ParameterId == publParam.ParameterId && lp.LabReportId == labReportId)
				select lp
			)
			.AsNoTracking().ToList();
		}

		public IEnumerable<LabReportParam> GetLabReportParamsByPublParam(PublParam publParam, Guid labReportId)
		{
			return
			(
				from lp in Context.LabReportParams
					.Where(lp => lp.ParameterId == publParam.ParameterId && lp.LabReportId == labReportId)
				join u in Context.Units on lp.UnitId equals u.UnitId
				join ppu in Context.Units on publParam.UnitId equals ppu.UnitId
				where (u.UnitName.Length > 0 &&
					ppu.UnitName.Length > 0 &&
					u.UnitName.Substring(1, u.UnitName.Length - 1).Equals(ppu.UnitName.Substring(1, ppu.UnitName.Length - 1))) ||
					(u.UnitName.Length == 0 && ppu.UnitName.Length == 0)
				select lp
			)
			.AsNoTracking().ToList();
		}
	}
}