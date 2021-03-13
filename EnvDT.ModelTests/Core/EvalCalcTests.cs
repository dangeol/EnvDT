using EnvDT.Model.Core;
using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace EnvDT.ModelTests.Core
{
    public class EvalCalcTests
    {
        private EvalCalc _evalCalc;

        private Mock<IUnitOfWork> _unitOfWorkMock;

        public EvalCalcTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _evalCalc = new EvalCalc(_unitOfWorkMock.Object);           
        }       

        [Theory]
        [InlineData(1.0, "µg/l", "µg/l", 1.0)]
        [InlineData(1.0, "µg/l", "mg/l", 0.001)]
        [InlineData(1.0, "mg/l", "µg/l", 1000)]
        public void SampleValueConversionShouldReturnCorrectValue(
            double sampleValue, string sampleValueUnitName, string refValUnitName, double expectedValue)
        {
            var calculatedValue = _evalCalc.SampleValueConversion(
                sampleValue, sampleValueUnitName, refValUnitName);

            Assert.Equal(calculatedValue, expectedValue);
        }

        [Theory]
        [InlineData(100.0, 100.0, "", false)]
        [InlineData(100.0, 99.9, "", true)]
        [InlineData(100.0, 99.9, "lower", false)]
        [InlineData(99.9, 100.0, "lower", true)]
        public void IsSampleValueExceedingRefValueShouldReturnCorrectValue(
            double sampleValue, double refVal, string refValParamAnnot, bool expectedValue)
        {
            var calculatedValue = _evalCalc.IsSampleValueExceedingRefValue(
                sampleValue, refVal, refValParamAnnot);

            Assert.Equal(calculatedValue, expectedValue);
        }

        [Theory]
        [MemberData(nameof(GetFinalSValuesObjects))]
        public void GetFinalSValueShouldReturnCorrectValue(
            EvalArgs evalArgs, 
            string refValParamAnnot, 
            List<KeyValuePair<LabReportParam, double>> LrParamSValuePairs, 
            FinalSValue expectedValue)
        {
            var calculatedValue = _evalCalc.GetFinalSValue(
                evalArgs, refValParamAnnot, LrParamSValuePairs);

            Assert.Equal(calculatedValue.LabReportParamName, expectedValue.LabReportParamName);
            Assert.Equal(calculatedValue.SValue, expectedValue.SValue);
        }

        public static IEnumerable<object[]> GetFinalSValuesObjects()
        {
            yield return new object[]
            { 
                new EvalArgs
                {
                    SelectSameLrParamMaxValue = false,
                    SelectDiffLrParamMaxValue = false
                },
                "",
                new List<KeyValuePair<LabReportParam, double>>
                {
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 2.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 5.0),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 7.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 10.0),
                },
                new FinalSValue
                {
                    LabReportParamName = "Param1",
                    SValue = 2.5
                }
            };
            yield return new object[]
            { 
                new EvalArgs
                {
                    SelectSameLrParamMaxValue = false,
                    SelectDiffLrParamMaxValue = true
                },
                "",
                new List<KeyValuePair<LabReportParam, double>>
                {
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 2.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 5.0),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 7.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 10.0),
                },
                new FinalSValue
                {
                    LabReportParamName = "",
                    SValue = 7.5
                }
            };
            yield return new object[]
            {
                new EvalArgs
                {
                    SelectSameLrParamMaxValue = true,
                    SelectDiffLrParamMaxValue = false
                },
                "",
                new List<KeyValuePair<LabReportParam, double>>
                {
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 2.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 5.0),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 7.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 10.0),
                },
                new FinalSValue
                {
                    LabReportParamName = "Param1",
                    SValue = 5.0
                }
            };
            yield return new object[]
            { 
                new EvalArgs
                {
                    SelectSameLrParamMaxValue = true,
                    SelectDiffLrParamMaxValue = true
                },
                "",
                new List<KeyValuePair<LabReportParam, double>>
                {
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 2.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 5.0),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 7.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 10.0),
                },
                new FinalSValue
                {
                    LabReportParamName = "",
                    SValue = 10.0
                }                
            };
            yield return new object[]
{
                new EvalArgs
                {
                    SelectSameLrParamMaxValue = true,
                    SelectDiffLrParamMaxValue = false
                },
                "lower",
                new List<KeyValuePair<LabReportParam, double>>
                {
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 2.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param1" }, 5.0),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 7.5),
                    new KeyValuePair<LabReportParam, double>(
                        new LabReportParam { LabReportParamName = "Param2" }, 10.0),
                },
                new FinalSValue
                {
                    LabReportParamName = "",
                    SValue = 10.0
                }                
            };
        }
    }
}
