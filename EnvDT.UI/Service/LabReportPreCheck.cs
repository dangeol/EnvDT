using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Settings.Localization;
using EnvDT.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.Model.Core
{
    public class LabReportPreCheck : ViewModelBase, ILabReportPreCheck
    {
        private IUnitOfWork _unitOfWork;
        private IMessageDialogService _messageDialogService;
        private HashSet<Guid> _missingParamIds;
        private HashSet<Guid> _missingUnitIds;
        private Func<IMissingParamDialogViewModel> _missingParamDialogVmCreator;
        private TranslationSource _translator = TranslationSource.Instance;
        private readonly IDispatcher _dispatcher;
        private IFootnotes _footnotes;

        public LabReportPreCheck(IUnitOfWork unitOfWork, IMessageDialogService messageDialogService,
            Func<IMissingParamDialogViewModel> missingParamDetailVmCreator, IDispatcher dispatcher,
            IFootnotes countrySpecific)
        {
            _unitOfWork = unitOfWork;
            _messageDialogService = messageDialogService;
            _missingParamIds = new HashSet<Guid>();
            _missingUnitIds = new HashSet<Guid>();
            _missingParamDialogVmCreator = missingParamDetailVmCreator;
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _footnotes = countrySpecific;
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
                    var isParamReallyMissing = true;
                    // If footnote existing, checking if condition is met that makes this param mandatory ("really missing")
                    if (publParam.FootnoteId != null)
                    {
                        EvalArgs evalArgs = new();
                        evalArgs.LabReportId = labReportId;
                        isParamReallyMissing = _footnotes.IsFootnoteCondTrue(evalArgs, publParam.FootnoteId).Result;
                    }

                    if (publParam.IsMandatory && !labReportParams.Any() && isParamReallyMissing)
                    {
                        var missingParameter = _unitOfWork.LabReportParams.GetLabReportParamNamesByPublParam(publParam, labReportId);
                        if (!labReportParams.Any())
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
                _dispatcher.Invoke(() =>
                {
                    var missingParamDialogVM = _missingParamDialogVmCreator();
                    missingParamDialogVM.Load(labReportId, _missingParamIds, _missingUnitIds);
                    var titleName = _translator["EnvDT.UI.Properties.Strings.LabReportPreCheck_DialogTitle_MissingParam"];
                    result = _messageDialogService.ShowMissingParamDialog(titleName, missingParamDialogVM);
                });
                return result == MessageDialogResult.OK;
            }
            return true;
        }
    }
}
