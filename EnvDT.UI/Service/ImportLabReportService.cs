﻿using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Settings.Localization;
using EnvDT.UI.ViewModel;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnvDT.UI.Service
{
    public class ImportLabReportService : IImportLabReportService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReadFileHelper _readFileHelper;
        private readonly List<Sample> _samples = new List<Sample>();
        private readonly TranslationSource _translator = TranslationSource.Instance;
        private readonly IDispatcher _dispatcher;

        public ImportLabReportService(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IUnitOfWork unitOfWork, IReadFileHelper readFileHelper, IDispatcher dispatcher)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _unitOfWork = unitOfWork;
            _readFileHelper = readFileHelper;
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void RunImport(string file, Guid? projectId)
        {
            ImportedFileData data = _readFileHelper.ReadFile(file);
            if (data != null)
            {
                ImportLabReport(data, projectId);
            }
        }

        private void ImportLabReport(ImportedFileData data, Guid? projectId)
        {
            DataTable workSheet = data.WorkSheet;
            ConfigBase configBase = new ConfigBase();
            var configId = data.ConfigId;
            var configType = data.ConfigType;
            string reportLabIdent = data.ReportLabIdent;
            switch (configType)
            {
                case "xls(x)":
                    configBase = _unitOfWork.ConfigXlsxs.GetById(configId);
                    break;

                case "csv":
                    configBase = data.ConfigCsv;
                    break;

                default:
                    _dispatcher.Invoke(() =>
                    {
                        _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogTitle_UnknLabRFormat"],
                        _translator["EnvDT.UI.Properties.Strings.ReadFileHelper_DialogMsg_UnknLabRFormat"]);
                    });
                    break;
            }

            if (IsLabReportAlreadyPresent(reportLabIdent))
            {
                return;
            }

            Laboratory laboratory = _unitOfWork.Laboratories.GetById(configBase.LaboratoryId);
            var labCompany = laboratory.LabCompany;
            Guid labReportId = CreateLabReport(reportLabIdent, laboratory.LaboratoryId, projectId).LabReportId;

            int c = configBase.FirstSampleValueCol;
            while (c < workSheet.Columns.Count)
            {
                string sampleLabIdent;
                try
                {
                    if (workSheet.Rows[configBase.SampleLabIdentRow][c] != System.DBNull.Value)
                    {
                        sampleLabIdent = workSheet.Rows[configBase.SampleLabIdentRow][c].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(sampleLabIdent));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _dispatcher.Invoke(() =>
                    {
                        _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "sampleLabIdent", ex.Message));
                    });
                    return;
                }
                string sampleName;
                try
                {
                    if (workSheet.Rows[configBase.SampleNameRow][c] != System.DBNull.Value)
                    {
                        sampleName = workSheet.Rows[configBase.SampleNameRow][c].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(sampleName));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _dispatcher.Invoke(() =>
                    {
                        _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "sampleName", ex.Message));
                    });
                    return;
                }

                var existingSample = _samples.FirstOrDefault(s => s.SampleName == sampleName);
                if (existingSample != null)
                {
                    existingSample.SampleLabIdent += "_" + sampleLabIdent;
                }
                else
                {
                    Sample sample = CreateSample(
                        sampleLabIdent, sampleName, labReportId
                    );
                    _samples.Add(sample);
                }
                c++;
            }
            CreateLabReportParams(workSheet, configBase, labReportId);

            try
            {
                _unitOfWork.Save();

                RaiseLabReportImportedEvent(labReportId,
                $"{reportLabIdent} {labCompany}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _dispatcher.Invoke(() =>
                {
                    _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_Error"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_Error"],
                    ex.Message));
                });
            }
        }

        public bool IsLabReportAlreadyPresent(string reportLabIdent)
        {
            var foundLabReport = _unitOfWork.LabReports.GetByReportLabIdent(reportLabIdent);
            if (foundLabReport != null)
            {
                _dispatcher.Invoke(() =>
                {
                    _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogTitle_ImportLabReport"],
                    _translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogMsg_ImportLabReport"]);
                });

                return true;
            }
            return false;
        }

        private LabReport CreateLabReport(string reportLabIdent, Guid laboratoryId, Guid? projectId)
        {
            var labReport = new LabReport();
            labReport.ReportLabIdent = reportLabIdent;
            labReport.LaboratoryId = laboratoryId;
            labReport.ProjectId = (Guid)projectId;

            _unitOfWork.LabReports.Create(labReport);

            return labReport;
        }

        private Sample CreateSample(string sampleLabIdent, string sampleName, Guid labReportId)
        {
            var sample = new Sample();
            sample.SampleLabIdent = sampleLabIdent;
            sample.SampleName = sampleName;
            sample.LabReportId = labReportId;

            _unitOfWork.Samples.Create(sample);

            return sample;
        }

        private void CreateLabReportParams(DataTable workSheet, ConfigBase configBase, Guid labReportId)
        {
            int r = configBase.FirstDataRow;
            while (r < workSheet.Rows.Count)
            {
                string labParamName;
                try
                {
                    if (workSheet.Rows[r][configBase.ParamNameCol] != System.DBNull.Value)
                    {
                        labParamName = workSheet.Rows[r][configBase.ParamNameCol].ToString();
                    }
                    else
                    {
                        DisplayReadingCellErrorMessage(nameof(labParamName));
                        return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _dispatcher.Invoke(() =>
                    {
                        _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "ParamName", ex.Message));
                    });
                    return;
                }
                var labParamUnitName = workSheet.Rows[r][configBase.UnitNameCol].ToString();
                var paramNameVariants = _unitOfWork.ParamNameVariants.GetParamNameVariantsByLabParamName(labParamName);
                var unitId = GetUnitNameVariantByLabParamUnitName(labParamUnitName);

                if (paramNameVariants.Count() > 0)
                {
                    foreach (var paramNameVariant in paramNameVariants)
                    {
                        var parameterId = paramNameVariant.ParameterId;
                        CreateNewLabParam(workSheet, configBase, labParamName, labParamUnitName,
                            parameterId, unitId, labReportId, r);
                    }
                }
                else
                {
                    var unknownParameterId = _unitOfWork.Parameters.GetParameterIdOfUnknown();
                    CreateNewLabParam(workSheet, configBase, labParamName, labParamUnitName,
                        unknownParameterId, unitId, labReportId, r);
                }
                r++;
            }
        }

        private Guid GetUnitNameVariantByLabParamUnitName(string labParamUnitName)
        {
            var unitName = labParamUnitName;
            var unitId = _unitOfWork.UnitNameVariants.GetUnitNameVariantByLabParamUnitName(unitName)?.UnitId
                ?? _unitOfWork.Units.GetUnitIdOfUnknown();

            return unitId;
        }

        // TO DO: refactoring
        private void CreateNewLabParam(DataTable workSheet, ConfigBase configBase, string labReportParamName,
            string labReportUnitName, Guid parameterId, Guid unitId, Guid labReportId, int r)
        {
            var labReportParam = new LabReportParam();
            labReportParam.ParameterId = parameterId;
            labReportParam.LabReportParamName = labReportParamName;
            labReportParam.LabReportUnitName = labReportUnitName;
            labReportParam.UnitId = unitId;
            labReportParam.LabReportId = labReportId;
            labReportParam.DetectionLimit = 0.0;

            try
            {
                if (workSheet.Rows[r][configBase.DetectionLimitCol] != System.DBNull.Value)
                {
                    double testVar;
                    var str = workSheet.Rows[r][configBase.DetectionLimitCol].ToString();
                    if (Double.TryParse(str, out testVar))
                    {
                        labReportParam.DetectionLimit = testVar;
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _dispatcher.Invoke(() =>
                {
                    _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                    "DetectionLimit", ex.Message));
                });
                return;
            }

            labReportParam.Method = "";

            try
            {
                if (workSheet.Rows[r][configBase.MethodCol] != System.DBNull.Value)
                {
                    labReportParam.Method = (string)workSheet.Rows[r][configBase.MethodCol];
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _dispatcher.Invoke(() =>
                {
                    _messageDialogService.ShowOkDialog(
                    _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                    string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                    "Method", ex.Message));
                });
                return;
            }

            _unitOfWork.LabReportParams.Create(labReportParam);
            CreateNewSampleValue(r, workSheet, configBase, labReportParam);
        }

        private void CreateNewSampleValue(int r, DataTable workSheet, ConfigBase configBase, LabReportParam labReportParam)
        {
            var c = configBase.FirstSampleValueCol;
            while (c < workSheet.Columns.Count)
            {
                try
                {
                    if (workSheet.Rows[r][c] != System.DBNull.Value)
                    {
                        var sValue = 0.0;
                        string sampleName;
                        Guid sampleId = Guid.Empty;

                        try
                        {
                            if (workSheet.Rows[configBase.SampleNameRow][c] != System.DBNull.Value)
                            {
                                sampleName = workSheet.Rows[configBase.SampleNameRow][c].ToString();
                                sampleId = _samples.FirstOrDefault(s => s.SampleName == sampleName).SampleId;
                            }
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            _dispatcher.Invoke(() =>
                            {
                                _messageDialogService.ShowOkDialog(
                                _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                                string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                                "SampleName", ex.Message));
                            });
                            return;
                        }

                        double testVar;
                        var str = workSheet.Rows[r][c].ToString();
                        if (Double.TryParse(str, out testVar))
                        {
                            sValue = testVar;
                        }
                        var sampleValue = new SampleValue();
                        sampleValue.SValue = sValue;
                        sampleValue.LabReportParamId = labReportParam.LabReportParamId;

                        if (sampleId != Guid.Empty)
                        {
                            sampleValue.SampleId = sampleId;
                            _unitOfWork.SampleValues.Create(sampleValue);
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    _dispatcher.Invoke(() =>
                    {
                        _messageDialogService.ShowOkDialog(
                        _translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_OutOfRangeEx"],
                        string.Format(_translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_OutOfRangeEx"],
                        "FirstSampleValue", ex.Message));
                    });
                    return;
                }
                c++;
            }
        }

        private void RaiseLabReportImportedEvent(Guid modelId, string displayMember)
        {
            _eventAggregator.GetEvent<LabReportImportedEvent>()
                .Publish(
                    new LabReportImportedEventArgs
                    {
                        Id = modelId,
                        DisplayMember = displayMember
                    });
        }

        private void DisplayReadingCellErrorMessage(string variableName)
        {
            _dispatcher.Invoke(() =>
            {
                _messageDialogService.ShowOkDialog(
                _translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogTitle_CellError"],
                string.Format(_translator["EnvDT.UI.Properties.Strings.ImportLabReportService_DialogMsg_CellError"],
                variableName));
            });
        }
    }
}
