using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EnvDT.Model.Core
{
    public class LabReportPreCheck : ILabReportPreCheck
    {
        private IUnitOfWork _unitOfWork;
        private IMessageDialogService _messageDialogService;
        private HashSet<Guid> _missingParamIds;
        private HashSet<Guid> _missingUnitIds;
        private Func<IMissingParamDialogViewModel> _missingParamDialogVmCreator;

        public LabReportPreCheck(IUnitOfWork unitOfWork, IMessageDialogService messageDialogService,
            Func<IMissingParamDialogViewModel> missingParamDetailVmCreator)
        {
            _unitOfWork = unitOfWork;
            _messageDialogService = messageDialogService;
            _missingParamIds = new HashSet<Guid>();
            _missingUnitIds = new HashSet<Guid>();
            _missingParamDialogVmCreator = missingParamDetailVmCreator;
        }

        public bool FindMissingParametersUnits(Guid labReportId, IReadOnlyCollection<Guid> publicationIds)
        {
            _missingParamIds.Clear();
            _missingUnitIds.Clear();

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
                        else if (missingParameter.First().UnitId == _unitOfWork.Units.GetUnitIdOfUnknown())
                        {
                            _missingUnitIds.Add(publParam.UnitId);                         
                        }
                    }
                }
            }
            if (_missingParamIds.Count > 0 || _missingUnitIds.Count > 0)
            {
                var result = MessageDialogResult.Cancel;
                Application.Current.Dispatcher.Invoke(() =>
                { 
                    var missingParamDialogVM = _missingParamDialogVmCreator();
                    missingParamDialogVM.Load(labReportId, _missingParamIds, _missingUnitIds);
                    var titleName = "Missing parameters";
                    result = _messageDialogService.ShowMissingParamDialog(titleName, missingParamDialogVM);
                });
                return result == MessageDialogResult.OK;
            }
            return true;
        }
    }
}
