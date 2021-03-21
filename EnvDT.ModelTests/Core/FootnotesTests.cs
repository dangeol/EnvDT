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
        private int _evalType;
        private string _footnoteRef;
        private Sample _sample;
        private SampleValue _sampleValue;
        private List<SampleValue> _sValuesFromLrParam;

        private SampleValueAndLrUnitName _sampleValueAndLrUnitName;
        private List<SampleValueAndLrUnitName> _sampleValueAndLrUnitNames;

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
            _sample = new();
            _sample.SampleId = new Guid();
            _sampleValue = new();
            _sValuesFromLrParam = new();
            _sValuesFromLrParam.Add(_sampleValue);

            _publParam = new();
            _labReportParams = new List<LabReportParam>();
            _labReportParam = new();
            _labReportParams.Add(_labReportParam);

            _unitOfWorkMock.Setup(uw => uw.PublParams.GetByPublIdParameterNameDeAndUnitName(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
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
        public void IsFootnoteCondTrueWithEvalType0ShouldReturnCorrectValue(
            double sValue, bool expectedResult)
        {
            _evalArgs.Sample = _sample;
            _evalType = 0;
            _footnoteRef = "VerfLfBY_e5";

            //Chrome Gesamt
            _sValue = sValue;

            _sampleValueAndLrUnitNames = new();
            
            _unitOfWorkMock.Setup(uw => uw.SampleValues.GetSampleValuesAndLrUnitNamesByLabReportIdParamNameDeAndUnitName(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sampleValueAndLrUnitNames);
            _evalCalcMock.Setup(ec => ec.SampleValueConversion(
                It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sValue);

            var calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _evalType, _footnoteRef).Result;
            // Should return false by default
            Assert.False(calculatedResult);

            _sampleValueAndLrUnitName = new();
            _sampleValueAndLrUnitName.sampleValue = _sampleValue;
            _sampleValue.SValue = sValue;
            _sampleValueAndLrUnitName.unitName = "µg/l";
            _sampleValueAndLrUnitNames.Add(_sampleValueAndLrUnitName);

            calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _evalType, _footnoteRef).Result;
            Assert.Equal(calculatedResult, expectedResult);
        }

        [Theory]
        [InlineData(30.1, 8.0, true)]
        [InlineData(30.1, 8.1, false)]
        [InlineData(50.0, 8.0, true)]
        [InlineData(50.1, 8.0, false)]
        public void IsFootnoteCondTrueWithEvalType1ShouldReturnCorrectValue(
            double sValue1, double sValue2, bool expectedResult)
        {
            _evalArgs.Sample = _sample;
            _evalType = 1;
            _footnoteRef = "VerfLfBY_e5";

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

            var calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _evalType, _footnoteRef);

            Assert.Equal(calculatedResult.Result, expectedResult);
            Assert.Equal(calculatedResult.TakingAccountOf.Count > 0, expectedResult);
        }

        [Theory]
        [InlineData(30.0, false)]
        [InlineData(30.1, true)]
        public void IsFootnoteCondTrueWithEvalType1ShouldReturnFalseAsDefaultAndMissingParamsWhenParamsAreMissing(
            double sValue, bool expectedResult)
        {
            _evalArgs.Sample = _sample;
            _evalType = 1;
            _footnoteRef = "VerfLfBY_e5";

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

            var calculatedResult = _footnotes.IsFootnoteCondTrue(_evalArgs, _evalType, _footnoteRef);

            Assert.False(calculatedResult.Result);
            //In this specific footnote: Only when condition for first parameter is met, missingParams is added to result
            Assert.Equal(calculatedResult.MissingParams.Count > 0, expectedResult);
        }
    }
}
