using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.Model.Core
{
    public class LabReportPreCheck : ILabReportPreCheck
    {
        private IUnitOfWork _unitOfWork;
        private HashSet<Guid> _missingParamIds;
        private Func<IMissingParamDialogViewModel> _missingParamDetailVmCreator;

        public LabReportPreCheck(IUnitOfWork unitOfWork, Func<IMissingParamDialogViewModel> missingParamDetailVmCreator)
        {
            _unitOfWork = unitOfWork;
            _missingParamIds = new HashSet<Guid>();
            _missingParamDetailVmCreator = missingParamDetailVmCreator;
        }

        public void FindMissingParametersUnits(Guid labReportId, IReadOnlyCollection<Guid> publicationIds)
        {
            foreach (Guid publicationId in publicationIds)
            {
                var publication = _unitOfWork.Publications.GetById(publicationId);
                var publParams = publication.PublParams;

                foreach (PublParam publParam in publParams)
                {
                    var labReportParams = _unitOfWork.LabReportParams.GetLabReportParamsByPublParam(publParam, labReportId);
                    if (labReportParams.Count() == 0)
                    {
                        var missingParameter = _unitOfWork.LabReportParams.GetLabReportParamNamesByPublParam(publParam, labReportId);
                        if (missingParameter.Count() == 0)
                        {
                            _missingParamIds.Add(publParam.ParameterId);
                        }
                    }
                    //TO DO: also pre check the units
                }
            }
            if (_missingParamIds.Count > 0)
            {
                var missingParamDetailVM = _missingParamDetailVmCreator();
                missingParamDetailVM.Load(_missingParamIds);
            }
        }
    }
}
