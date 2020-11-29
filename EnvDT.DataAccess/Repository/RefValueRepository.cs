using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class RefValueRepository : GenericRepository<RefValue, EnvDTDbContext>,
        IRefValueRepository
    {
        public RefValueRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<RefValue> GetRefValuesByPublParamIdAndSample(Guid publParamId, Sample sample)
        {
			return
			(
				from rv in Context.RefValues
					.Where(rv => rv.PublParamId == publParamId)
				join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
				where (vc.ValuationClassMedSubTypes.Count == 0
						&& vc.ValuationClassConditions.Count == 0)
				select rv
			)
			.AsNoTracking().ToList();
		}

		public IEnumerable<RefValue> GetRefValuesWithMedSubTypesByPublParamIdAndSample(Guid publParamId, Sample sample)
		{
			var refValues = new List<RefValue>();

			var rValuesWithoutAttributes =
			(
				from rv in Context.RefValues
					.Where(rv => rv.PublParamId == publParamId)
				join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
				where (vc.ValuationClassMedSubTypes.Count == 0
						&& vc.ValuationClassConditions.Count == 0)
				select rv
			)
			.AsNoTracking();
			refValues.AddRange(rValuesWithoutAttributes);

			if (sample.MediumSubTypeId != null && sample.ConditionId == null)
			{
				var rvalues =
				(
					from rv in Context.RefValues
						.Where(rv => rv.PublParamId == publParamId)
					join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
					where (vc.ValuationClassConditions.Count == 0)
					join vcmst in Context.ValuationClassMedMedSubTypes on vc.ValuationClassId equals vcmst.ValuationClassId
					where (vcmst.MedSubTypeId == sample.MediumSubTypeId)
					select rv
				)
				.AsNoTracking();
				refValues.AddRange(rvalues);
			}

			return refValues;
		}

		public IEnumerable<RefValue> GetRefValuesWithConditionsByPublParamIdAndSample(Guid publParamId, Sample sample)
		{
			var refValues = new List<RefValue>();

			var rValuesWithoutAttributes =
			(
				from rv in Context.RefValues
					.Where(rv => rv.PublParamId == publParamId)
				join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
				where (vc.ValuationClassMedSubTypes.Count == 0
						&& vc.ValuationClassConditions.Count == 0)
				select rv
			)
			.AsNoTracking();
			refValues.AddRange(rValuesWithoutAttributes);

			if (sample.MediumSubTypeId == null && sample.ConditionId != null)
			{
				var rvalues =
				(
					from rv in Context.RefValues
						.Where(rv => rv.PublParamId == publParamId)
					join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
					where (vc.ValuationClassMedSubTypes.Count == 0)
					join vcc in Context.ValuationClassConditions on vc.ValuationClassId equals vcc.ValuationClassId
					where (vcc.ConditionId == sample.ConditionId)
					select rv
				)
				.AsNoTracking();
				refValues.AddRange(rvalues);
			}

			return refValues;
		}

		public IEnumerable<RefValue> GetRefValuesWithMedSubTypesAndConditionsByPublParamIdAndSample(Guid publParamId, Sample sample)
		{
			var refValues = new List<RefValue>();

			var rValuesWithoutAttributes =
			(
				from rv in Context.RefValues
					.Where(rv => rv.PublParamId == publParamId)
				join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
				where (vc.ValuationClassMedSubTypes.Count == 0
						&& vc.ValuationClassConditions.Count == 0)
				select rv
			)
			.AsNoTracking();
			refValues.AddRange(rValuesWithoutAttributes);

			if (sample.MediumSubTypeId != null)
			{
				var rvalues =
				(
					from rv in Context.RefValues
						.Where(rv => rv.PublParamId == publParamId)
					join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
					where (vc.ValuationClassConditions.Count == 0)
					join vcmst in Context.ValuationClassMedMedSubTypes on vc.ValuationClassId equals vcmst.ValuationClassId
					where (vcmst.MedSubTypeId == sample.MediumSubTypeId)
					select rv
				)
				.AsNoTracking();
				refValues.AddRange(rvalues);
			}
			if (sample.ConditionId != null)
			{
				var rvalues =
				(
					from rv in Context.RefValues
						.Where(rv => rv.PublParamId == publParamId)
					join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
					where (vc.ValuationClassMedSubTypes.Count == 0)
					join vcc in Context.ValuationClassConditions on vc.ValuationClassId equals vcc.ValuationClassId
					where (vcc.ConditionId == sample.ConditionId)
					select rv
				)
				.AsNoTracking();
				refValues.AddRange(rvalues);
			}
			if (sample.MediumSubTypeId != null && sample.ConditionId != null)
			{
				var rvalues =
				(
					from rv in Context.RefValues
						.Where(rv => rv.PublParamId == publParamId)
					join vc in Context.ValuationClasses on rv.ValuationClassId equals vc.ValuationClassId
					join vcmst in Context.ValuationClassMedMedSubTypes on vc.ValuationClassId equals vcmst.ValuationClassId
					join vcc in Context.ValuationClassConditions on vc.ValuationClassId equals vcc.ValuationClassId
					where (vcmst.MedSubTypeId == sample.MediumSubTypeId && vcc.ConditionId == sample.ConditionId)
					select rv
				)
				.AsNoTracking();
				refValues.AddRange(rvalues);
			}

			return refValues;
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
	}
}

