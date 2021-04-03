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
    public class EvalLabReportServiceTests
    {
        private EvalLabReportService _evalLabReportService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILabReportPreCheck> _labReportPreCheck;
        private Mock<IEvalCalc> _evalCalcMock;
        private Mock<IFootnotes> _footnotesMock;
        private Sample _sample;
        private Publication _publication;
        private PublParam _publParam;
        private List<PublParam> _publParams;
        private RefValue _refValue;
        private List<RefValue> _refValues;
        private EvalArgs _evalArgs;

        private SampleValue _sampleValue1;
        private SampleValue _sampleValue2;
        private FinalSValue _finalSValue;
        private Unit _unit;
        private Parameter _parameter;
        private ValuationClass _valuationClass1;
        private ValuationClass _valuationClass2;
        private string _nextLevelName1 = "NextLevelName1";
        private string _nextLevelName2 = "NextLevelName2";
        private List<LabReportParam> _labReportParams;
        private LabReportParam _labReportParam;
        private List<KeyValuePair<LabReportParam, double>> _lrParamSValuePairs;

        public EvalLabReportServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _labReportPreCheck = new Mock<ILabReportPreCheck>();
            _evalCalcMock = new Mock<IEvalCalc>();
            _footnotesMock = new Mock<IFootnotes>();
            _publParam = new PublParam();
            _publParam.IsMandatory = true;
            _publParam.FootnoteId = "";
            _publParams = new List<PublParam>();
            _publParams.Add(_publParam);
            _refValue = new RefValue();
            _refValue.RValue = 5.0;
            _refValues = new List<RefValue>();
            _refValues.Add(_refValue);
            _publication = new Publication();
            _publication.PublicationId = new Guid();
            _publication.PublParams = _publParams;

            _sample = new Sample();
            _sample.SampleId = new Guid();
            _sample.SampleName = "Sample1";
            _labReportParams = new List<LabReportParam>();
            _labReportParam = new LabReportParam();
            _labReportParams.Add(_labReportParam);

            _evalArgs = new EvalArgs();
            _evalArgs.LabReportId = It.IsAny<Guid>();
            _evalArgs.Sample = _sample;
            _evalArgs.PublicationId = _publication.PublicationId;
            _evalArgs.EvalFootnotes = true;

            _unitOfWorkMock.Setup(uw => uw.Samples.GetById(It.IsAny<Guid>()))
                .Returns(It.IsAny<Sample>());
            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);
            _unitOfWorkMock.Setup(uw => uw.RefValues.GetRefValuesWithoutAttributesByPublParamId(It.IsAny<Guid>()))
                .Returns(_refValues);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetValClassNameNextLevelFromLevel(
                It.Is<int>(i => i == 0), It.IsAny<Guid>()))
                .Returns(_nextLevelName1);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetValClassNameNextLevelFromLevel(
                It.Is<int>(i => i == 1), It.IsAny<Guid>()))
                .Returns(_nextLevelName2);

            // GetExceedingValue method
            _sampleValue1 = new SampleValue();
            _sampleValue1.SValue = 10.0;
            _sampleValue2 = new SampleValue();
            _sampleValue2.SValue = 12.5;
            _lrParamSValuePairs = new List<KeyValuePair<LabReportParam, double>>();
            _lrParamSValuePairs.Add(new KeyValuePair<LabReportParam, double>(_labReportParam, _sampleValue1.SValue));
            _finalSValue = new FinalSValue();
            _finalSValue.LabReportParamName = "";
            _finalSValue.SValue = _sampleValue1.SValue;
            _unit = new Unit();
            _unit.UnitName = "mg/kg";
            _parameter = new Parameter();
            _valuationClass1 = new ValuationClass();
            _valuationClass1.ValClassLevel = 1;
            _valuationClass1.ValuationClassName = "LevelName1";
            _valuationClass2 = new ValuationClass();
            _valuationClass2.ValClassLevel = 2;
            _valuationClass2.ValuationClassName = "LevelName2";

            _unitOfWorkMock.Setup(uw => uw.Units.GetById(It.IsAny<Guid>()))
                .Returns(_unit);
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(It.IsAny<Guid>()))
                .Returns(_parameter);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetById(It.IsAny<Guid>()))
                .Returns(_valuationClass1);

            _evalCalcMock.Setup(ec => ec.SampleValueConversion(
                _sampleValue1.SValue, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sampleValue1.SValue);
            _evalCalcMock.Setup(ec => ec.GetLrParamSValuePairs(
                It.IsAny<IEnumerable<LabReportParam>>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(_lrParamSValuePairs);
            _evalCalcMock.Setup(ec => ec.GetFinalSValue(
                It.IsAny<EvalArgs>(), It.IsAny<string>(), It.IsAny<List<KeyValuePair<LabReportParam, double>>>()))
                .Returns(_finalSValue);

            // Instantiate class 
            _evalLabReportService = new EvalLabReportService(_unitOfWorkMock.Object,
                _labReportPreCheck.Object, _evalCalcMock.Object, _footnotesMock.Object);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void GetEvalResultShouldReturnCorrectEvalResult(
            bool isSampleValueExceedingRefValue, bool expectedResult)
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);
            _evalCalcMock.Setup(ec => ec.IsSampleValueExceedingRefValue(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
                .Returns(isSampleValueExceedingRefValue);

            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);

            Assert.Equal(evalResult.SampleName, _sample.SampleName);
            Assert.Equal(expectedResult, evalResult.ExceedingValues.Length > 0);
        }

        [Theory]
        [InlineData(false, false, "NextLevelName2")]
        [InlineData(false, true, "NextLevelName2")]
        [InlineData(true, false, "NextLevelName2")]
        [InlineData(true, true, "NextLevelName1")]
        public void ShouldNotIncreaseValuationClassLevelIfFootnotesAreEvaluatedAndFootnoteResultHasIsNotExclusionCriterion(
            bool isNotExclusionCriterion, bool evalFootnotes, string expectedResult)
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);
            _evalCalcMock.Setup(ec => ec.IsSampleValueExceedingRefValue(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
                .Returns(true);

            _evalArgs.EvalFootnotes = evalFootnotes;
            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            HashSet<string> generalFootnoteTexts = new();
            evalResult.GeneralFootnoteTexts = generalFootnoteTexts;
            
            _publParam.FootnoteId = "footnoteId";
            _refValue.FootnoteId = "";

            FootnoteResult footnoteResult = new()
            {
                Result = true,
                IsNotExclusionCriterion = isNotExclusionCriterion,
                GeneralFootnoteTexts = "Footnote1 text"
            };

            _footnotesMock.Setup(fm => fm.IsFootnoteCondTrue(It.IsAny<EvalArgs>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(footnoteResult);

            evalResult = _evalLabReportService.GetEvalResult(_evalArgs);

            Assert.Equal(expectedResult, evalResult.HighestValClassName);
        }

        [Fact]
        public void ShouldAddGeneralFootnoteTextsIfPublParamHasFootnoteIdAndFootnoteResultHasFootnoteText()
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetValClassNameNextLevelFromLevel(
                It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns(_nextLevelName1);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetById(It.IsAny<Guid>()))
                .Returns(_valuationClass1);
            _evalCalcMock.Setup(ec => ec.IsSampleValueExceedingRefValue(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
                .Returns(true);

            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            HashSet<string> generalFootnoteTexts = new();
            evalResult.GeneralFootnoteTexts = generalFootnoteTexts;
            var generalFootnoteTextsLengthBefore = evalResult.GeneralFootnoteTexts.Count;

            _publParam.FootnoteId = "footnoteId";
            _refValue.FootnoteId = "";

            FootnoteResult footnoteResult = new()
            {
                Result = true,
                IsNotExclusionCriterion = true,
                GeneralFootnoteTexts = "Footnote1 text"
            };

            _footnotesMock.Setup(fm => fm.IsFootnoteCondTrue(It.IsAny<EvalArgs>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(footnoteResult);

            evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var generalFootnoteTextsLengthAfter = evalResult.GeneralFootnoteTexts.Count;

            Assert.True(generalFootnoteTextsLengthAfter > generalFootnoteTextsLengthBefore);
        }        

        [Fact]
        public void EvalResultShouldHaveMissingParamsWhenParamNotFound()
        {
            var labReportParams = new List<LabReportParam>();
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(labReportParams);
            _evalCalcMock.Setup(ec => ec.IsSampleValueExceedingRefValue(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
                .Returns(false);

            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);

            Assert.True(evalResult.MissingParams.Length > 0);
        }

        [Fact]
        public void ShouldAddMissingParamsWhenNoSampleValueFound()
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);
            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var missingParamsLengthBefore = evalResult.MissingParams.Length;

            _lrParamSValuePairs.Clear();
            evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var missingParamsLengthAfter = evalResult.MissingParams.Length;

            Assert.True(missingParamsLengthAfter > missingParamsLengthBefore);
        }

        [Fact]
        public void ShouldAddMinValueParamsWhenEvalResultHasMinValueParams()
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);

            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var minValueParamsLengthBefore = evalResult.MinValueParams.Length;
            
            _finalSValue.LabReportParamName = "param";
            _lrParamSValuePairs.Add(new KeyValuePair<LabReportParam, double>(_labReportParam, _sampleValue2.SValue));
            evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var minValueParamsLengthAfter = evalResult.MinValueParams.Length;

            Assert.True(minValueParamsLengthAfter > minValueParamsLengthBefore);
        }

        [Fact]
        public void ShouldAddTakingAccountOfFootnoteRefsWhenRefValHasRefValAltAndEvalResultHasTakingAccountOf()
        {
            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);

            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var takingAccountOfListLengthBefore = evalResult.TakingAccountOf.Length;

            _refValue.RValueAlt = 100;
            _refValue.FootnoteId = "";
            HashSet<PublParam> missingParams = new()
            {
                new PublParam()
            };
            HashSet<string> takingAccountOf = new();
            takingAccountOf.Add("Footnote1");
            FootnoteResult footnoteResult = new()
            {
                Result = true,
                MissingParams = missingParams,
                TakingAccountOf = takingAccountOf
            };

            _footnotesMock.Setup(fm => fm.IsFootnoteCondTrue(It.IsAny<EvalArgs>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(footnoteResult);
            
            evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var takingAccountOfListLengthAfter = evalResult.TakingAccountOf.Length;

            Assert.True(takingAccountOfListLengthAfter > takingAccountOfListLengthBefore);
        }
    }
}
