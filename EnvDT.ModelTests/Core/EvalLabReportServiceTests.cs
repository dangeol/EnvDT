using EnvDT.Model.Core;
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
        private Mock<IEvalCalcService> _evalCalcServiceMock;
        private Sample _sample;
        private Publication _publication;
        private List<RefValue> _refValues;
        private RefValue _refValue;

        private List<SampleValue> _sampleValues;
        private SampleValue _sampleValue;
        private Unit _unit;
        private Parameter _parameter;
        private ValuationClass _valuationClass;

        public EvalLabReportServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _evalCalcServiceMock = new Mock<IEvalCalcService>();
            _refValue = new RefValue();
            _refValue.RValue = 5.0;
            _refValues = new List<RefValue>();
            _refValues.Add(_refValue);

            _sample = new Sample();
            _sample.SampleId = new Guid();
            _sample.SampleName = "Sample1";
            _publication = new Publication();
            _publication.PublicationId = new Guid();

            _unitOfWorkMock.Setup(uw => uw.Samples.GetById(It.IsAny<Guid>()))
                .Returns(_sample);
            _unitOfWorkMock.Setup(uw => uw.Publications.GetById(It.IsAny<Guid>()))
                .Returns(_publication);
            _unitOfWorkMock.Setup(uw => uw.RefValues.GetRefValuesByPublicationId(It.IsAny<Guid>()))
                .Returns(_refValues);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.getValClassNameNextLevelFromLevel(
                It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns("Level");

            // GetExceedingValue method
            _sampleValue = new SampleValue();
            _sampleValue.SValue = 10.0;
            _sampleValues = new List<SampleValue>();
            _sampleValues.Add(_sampleValue);
            _unit = new Unit();
            _unit.UnitName = "mg/kg";
            _parameter = new Parameter();
            _valuationClass = new ValuationClass();
            _valuationClass.ValClassLevel = 1;
            _valuationClass.ValuationClassName = "ValClass1";

            _unitOfWorkMock.Setup(uw => uw.SampleValues.GetSampleValuesBySampleIdAndRefValue(
                It.IsAny<Guid>(), It.IsAny<RefValue>()))
                .Returns(_sampleValues);
            _unitOfWorkMock.Setup(uw => uw.Units.GetById(It.IsAny<Guid>()))
                .Returns(_unit);
            _unitOfWorkMock.Setup(uw => uw.Parameters.GetById(It.IsAny<Guid>()))
                .Returns(_parameter);
            _unitOfWorkMock.Setup(uw => uw.ValuationClasses.GetById(It.IsAny<Guid>()))
                .Returns(_valuationClass);

            _evalCalcServiceMock.Setup(ec => ec.SampleValueConversion(
                It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_sampleValue.SValue);

            // Instantiate class 
            _evalLabReportService = new EvalLabReportService(_unitOfWorkMock.Object, _evalCalcServiceMock.Object);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void GetEvalResultShouldReturnCorrectEvalResult(
            bool isSampleValueExceedingRefValue, bool expectedResult)
        {
            _evalCalcServiceMock.Setup(ec => ec.IsSampleValueExceedingRefValue(
                It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>()))
                .Returns(isSampleValueExceedingRefValue);

            var evalResult = _evalLabReportService.getEvalResult(
                _sample.SampleId, _publication.PublicationId);

            Assert.Equal(evalResult.SampleName, _sample.SampleName);
            Assert.Equal(expectedResult, evalResult.HighestValClassName == _valuationClass.ValuationClassName);
            Assert.Equal(expectedResult, evalResult.ExceedingValueList.Length > 0);
        }
    }
}
