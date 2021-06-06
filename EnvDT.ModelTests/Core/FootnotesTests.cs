using EnvDT.Model.Core;
using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EnvDT.ModelTests.Core
{
    public class FootnotesTests
    {
        private Footnotes _footnotes;

        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IEvalCalc> _evalCalcMock;

        private EvalArgs _evalArgs;
        private Footnote _footnote;
        private List<FootnoteParam> _footnoteParams;
        private FootnoteParam _footnoteParam1;
        private FootnoteParam _footnoteParam2;
        private Sample _sample;
        private SampleValue _sampleValue;
        private List<SampleValue> _sValuesFromLrParam;

        private SampleValueAndLrUnitName _sampleValueAndLrUnitName;
        private List<SampleValueAndLrUnitName> _sampleValueAndLrUnitNames;

        private Parameter _parameter;
        private Unit _unit;
        private PublParam _publParam;
        private List<LabReportParam> _labReportParams;
        private LabReportParam _labReportParam;
        private List<KeyValuePair<LabReportParam, double>> _lrParamSValuePairs1;
        private List<KeyValuePair<LabReportParam, double>> _lrParamSValuePairs2;
        private FinalSValue _finalSValue1;
        private FinalSValue _finalSValue2;
        private double _sValue;
        private double _sValue2;

        public FootnotesTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _evalCalcMock = new Mock<IEvalCalc>();

            _evalArgs = new();
            _footnote = new();
            _footnote.FootnoteId = new Guid("06c38588-53b8-4953-9ba4-85d0afb445d5");
            _footnote.Expression1 = "param1 > 30 AND param1 <= 50";

            _footnoteParam1 = new();
            _footnoteParam1.FootnoteId = new Guid("06c38588-53b8-4953-9ba4-85d0afb445d5");
            _footnoteParam2 = new();
            _footnoteParam2.FootnoteId = new Guid("06c38588-53b8-4953-9ba4-85d0afb445d5");
            _footnoteParams = new List<FootnoteParam>();

            _sample = new();
            _sample.SampleId = new Guid();
            _sampleValue = new();
            _sValuesFromLrParam = new();
            _sValuesFromLrParam.Add(_sampleValue);

            _parameter = new();
            _parameter.ParamAnnotation = "";
            _unit = new();
            _unit.UnitName = "µg/l";
            _publParam = new();
            _labReportParams = new List<LabReportParam>();
            _labReportParam = new();
            _labReportParams.Add(_labReportParam);

            _lrParamSValuePairs1 = new();

            _unitOfWorkMock.Setup(uw => uw.Footnotes.GetById(It.IsAny<Guid>()))
                .Returns(_footnote);
            _unitOfWorkMock.Setup(uw => uw.FootnoteParams.GetFootnoteParamsByFootnoteId(It.IsAny<Guid>()))
                .Returns(_footnoteParams);
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(It.IsAny<Guid>()))
                .Returns(_parameter);
            _unitOfWorkMock.Setup(uw => uw.Units.GetById(It.IsAny<Guid>()))
                .Returns(_unit);
            _unitOfWorkMock.Setup(uw => uw.PublParams.GetById(It.IsAny<Guid>()))
                .Returns(_publParam);
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(
                It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);

            _footnotes = new Footnotes(_unitOfWorkMock.Object, _evalCalcMock.Object);
        }

        // These test cases just deal with one example of many different footnote evaluation cases
        [Theory]
        [InlineData(30.0, false)]
        [InlineData(30.1, true)]
        [InlineData(50.0, true)]
        [InlineData(50.1, false)]
        public void IsFootnoteCondTrueWithEvalTypeLabReportPreCheckShouldReturnCorrectValue(
            double sValue, bool expectedResult)
        {
            _footnoteParams.Add(_footnoteParam1);
            _footnote.Expression2 = "";

            //Chrome Gesamt
            _sValue = sValue;

            _sampleValueAndLrUnitNames = new();

            _unitOfWorkMock.Setup(uw => uw.SampleValues.GetSampleValuesAndLrUnitNamesByLabReportIdParameterIdAndUnitName(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(_sampleValueAndLrUnitNames);
            _evalCalcMock.Setup(ec => ec.SampleValueConversion(
                It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sValue);

            var calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _footnote.FootnoteId).Result;
            // Should return false by default
            Assert.False(calculatedResult);

            _sampleValueAndLrUnitName = new();
            _sampleValueAndLrUnitName.sampleValue = _sampleValue;
            _sampleValue.SValue = sValue;
            _sampleValueAndLrUnitName.unitName = "µg/l";
            _sampleValueAndLrUnitNames.Add(_sampleValueAndLrUnitName);

            calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _footnote.FootnoteId).Result;
            Assert.Equal(calculatedResult, expectedResult);
        }

        [Theory]
        [InlineData(30.1, 8.0, true)]
        [InlineData(30.1, 8.1, false)]
        [InlineData(50.0, 8.0, true)]
        [InlineData(50.1, 8.0, false)]
        public void IsFootnoteCondTrueWithEvalTypeEvalLabReportServiceShouldReturnCorrectValue(
            double sValue1, double sValue2, bool expectedResult)
        {
            _evalArgs.Sample = _sample;
            _footnoteParams.Add(_footnoteParam1);
            _footnoteParams.Add(_footnoteParam2);
            _footnote.Expression2 = "param2 <= 8";

            //Chrome Gesamt
            _sValue = sValue1;
            //Cr VI
            _sValue2 = sValue2;

            _lrParamSValuePairs1 = new();
            _lrParamSValuePairs1.Add(new KeyValuePair<LabReportParam, double>(_labReportParam, _sValue));
            _lrParamSValuePairs2 = new();
            _lrParamSValuePairs2.Add(new KeyValuePair<LabReportParam, double>(_labReportParam, _sValue2));

            _finalSValue1 = new();
            _finalSValue1.SValue = _sValue;
            _finalSValue2 = new();
            _finalSValue2.SValue = _sValue2;

            _evalCalcMock.SetupSequence(ec => ec.GetLrParamSValuePairs(
                It.IsAny<IEnumerable<LabReportParam>>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(_lrParamSValuePairs1)
                .Returns(_lrParamSValuePairs2);
            // The next one below won't be called except if _lrParamSValuePairsX.Count() > 1
            _evalCalcMock.SetupSequence(ec => ec.GetFinalSValue(
                It.IsAny<EvalArgs>(), It.IsAny<string>(), It.IsAny<List<KeyValuePair<LabReportParam, double>>>()))
                .Returns(_finalSValue1)
                .Returns(_finalSValue2);

            var calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _footnote.FootnoteId);

            Assert.Equal(calculatedResult.Result, expectedResult);
            Assert.Equal(calculatedResult.TakingAccountOf.Count > 0, expectedResult);
        }

        /* Below test case is not valid anymore after db model restructuring regarding footnotes; but the underlying specific feature is on the TO DO list.
        [Theory]
        [InlineData(30.0, false)]
        [InlineData(30.1, true)]
        public void IsFootnoteCondTrueWithEvalType1ShouldReturnFalseAsDefaultAndMissingParamsWhenParamsAreMissing(
            double sValue, bool expectedResult)
        {
            _evalArgs.Sample = _sample;

            //Chrom Gesamt
            _sValue = sValue;

            var labReportParamsEmpty = new List<LabReportParam>();

            _unitOfWorkMock.SetupSequence(uw => uw.LabReportParams.GetLabReportParamsByPublParam(
                It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams)
                .Returns(labReportParamsEmpty);

            _lrParamSValuePairs1 = new();
            _lrParamSValuePairs1.Add(new KeyValuePair<LabReportParam, double>(_labReportParam, _sValue)); 

            _finalSValue1 = new();
            _finalSValue1.SValue = _sValue;

            _evalCalcMock.Setup(ec => ec.GetLrParamSValuePairs(
                It.IsAny<IEnumerable<LabReportParam>>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(_lrParamSValuePairs1);

            var calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _footnote.FootnoteId);

            Assert.False(calculatedResult.Result);
            //In this specific footnote: Only when condition for first parameter is met, missingParams is added to result
            Assert.Equal(calculatedResult.MissingParams.Count > 0, expectedResult);
        }
        */
    }
}
