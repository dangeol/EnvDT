using EnvDT.Model.Core;
using Xunit;

namespace EnvDT.ModelTests.Core
{
    public class EvalCalcTests
    {
        private EvalCalc _evalCalc;

        public EvalCalcTests()
        {
            _evalCalc = new EvalCalc();
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
    }
}
