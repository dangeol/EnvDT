﻿using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class LabReportParamRepository : GenericRepository<LabReportParam, EnvDTDbContext>,
        ILabReportParamRepository
    {
        public LabReportParamRepository(EnvDTDbContext context)
            :base(context)
        {
        }

		public IEnumerable<LabReportParam> GetLabReportParamsByPublParam(PublParam publParam)
		{
			return
			(
				from lp in Context.LabReportParams
					.Where(lp => lp.ParameterId == publParam.ParameterId)
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
	}
}