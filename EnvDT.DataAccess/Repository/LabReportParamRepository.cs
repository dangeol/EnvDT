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

		public IEnumerable<LabReportParam> GetLabReportParamNamesByPublParam(PublParam publParam, Guid labReportId)
		{
			return
			(
				from lp in Context.LabReportParams
					.Where(lp => lp.ParameterId == publParam.ParameterId && lp.LabReportId == labReportId)
				select lp
			)
			.ToList();
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
			.ToList();
		}

		public IEnumerable<LabReportParam> GetLabReportUnknownParamNamesByLabReportId(Guid labReportId)
		{
			var unknownParamNameId = Context.Parameters.AsNoTracking()
				.Single(p => p.ParamNameEn == "[unknown]").ParameterId;

			return Context.Set<LabReportParam>().AsNoTracking().ToList()
				.Where(lp => lp.ParameterId == unknownParamNameId && lp.LabReportId == labReportId);
		}

		public IEnumerable<LabReportParam> GetLabReportUnknownUnitNamesByLabReportId(Guid labReportId)
		{
			var unknownUnitNameId = Context.Units.AsNoTracking()
				.Single(p => p.UnitName == "[unknown]").UnitId;

			return Context.Set<LabReportParam>().AsNoTracking().ToList()
				.Where(lp => lp.UnitId == unknownUnitNameId && lp.LabReportId == labReportId);
		}
	}
}