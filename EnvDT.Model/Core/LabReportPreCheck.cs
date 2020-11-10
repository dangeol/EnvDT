using EnvDT.Model.IRepository;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public class LabReportPreCheck : ILabReportPreCheck
    {
        private IUnitOfWork _unitOfWork;
        public LabReportPreCheck(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void FindMissingParametersUnits(Guid labReportId, IReadOnlyCollection<Guid> publicationIds)
        {
            System.Diagnostics.Debug.WriteLine(labReportId + " - " + publicationIds.Count.ToString());
        }
    }
}
