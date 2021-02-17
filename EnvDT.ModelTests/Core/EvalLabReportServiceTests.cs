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
        private Sample _sample;
        private Publication _publication;
        private PublParam _publParam;
        private List<PublParam> _publParams;
        private RefValue _refValue;
        private List<RefValue> _refValues;
        private EvalArgs _evalArgs;

        private List<SampleValue> _sampleValues;
        private SampleValue _sampleValue1;
        private SampleValue _sampleValue2;
        private Unit _unit;
        private Parameter _parameter;
        private ValuationClass _valuationClass;
        private string _nextLevelName = "NextLevelName";
        private List<LabReportParam> _labReportParams;
        private LabReportParam _labReportParam;

        public EvalLabReportServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _labReportPreCheck = new Mock<ILabReportPreCheck>();
            _evalCalcMock = new Mock<IEvalCalc>();
            _publParam = new PublParam();
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

            _unitOfWorkMock.Setup(uw => uw.Samples.GetById(It.IsAny<Guid>()))
                .Returns(It.IsAny<Sample>());
            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);
            _unitOfWorkMock.Setup(uw => uw.RefValues.GetRefValuesWithoutAttributesByPublParamId(It.IsAny<Guid>()))
                .Returns(_refValues);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.getValClassNameNextLevelFromLevel(
                It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns(_nextLevelName);

            // GetExceedingValue method
            _sampleValue1 = new SampleValue();
            _sampleValue1.SValue = 10.0;
            _sampleValues = new List<SampleValue>();
            _sampleValues.Add(_sampleValue1);
            _unit = new Unit();
            _unit.UnitName = "mg/kg";
            _parameter = new Parameter();
            _valuationClass = new ValuationClass();
            _valuationClass.ValClassLevel = 1;
            _valuationClass.ValuationClassName = "LevelName";

            _unitOfWorkMock.Setup(uw => uw.SampleValues.GetSampleValuesBySampleIdAndLabReportParamId(
                It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(_sampleValues);
            _unitOfWorkMock.Setup(uw => uw.Units.GetById(It.IsAny<Guid>()))
                .Returns(_unit);
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(It.IsAny<Guid>()))
                .Returns(_parameter);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetById(It.IsAny<Guid>()))
                .Returns(_valuationClass);

            _evalCalcMock.Setup(ec => ec.SampleValueConversion(
                _sampleValue1.SValue, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sampleValue1.SValue);

            // Instantiate class 
            _evalLabReportService = new EvalLabReportService(_unitOfWorkMock.Object,
                _labReportPreCheck.Object, _evalCalcMock.Object);
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

            _sampleValues.Clear();
            evalResult = _evalLabReportService.GetEvalResult(_evalArgs);
            var missingParamsLengthAfter = evalResult.MissingParams.Length;

            Assert.True(missingParamsLengthAfter > missingParamsLengthBefore);
        }

        [Fact]
        public void ShouldOutputMaxExceedingSampleValueWhenSampleParamHasTwoSampleValues()
        {
            _sampleValue2 = new SampleValue();
            _sampleValue2.SValue = 20.1;
            _sampleValues.Add(_sampleValue2);

            _evalCalcMock.Setup(ec => ec.SampleValueConversion(
                _sampleValue2.SValue, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sampleValue2.SValue);
            _evalCalcMock.Setup(ec => ec.IsSampleValueExceedingRefValue(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
                .Returns(true);

            _unitOfWorkMock.Setup(uw => uw.LabReportParams.GetLabReportParamsByPublParam(It.IsAny<PublParam>(), It.IsAny<Guid>()))
                .Returns(_labReportParams);
            var evalResult = _evalLabReportService.GetEvalResult(_evalArgs);

            Assert.Contains(_sampleValue2.SValue.ToString(), evalResult.ExceedingValues);
            Assert.DoesNotContain(_sampleValue1.SValue.ToString(), evalResult.ExceedingValues);
        }
    }
}
