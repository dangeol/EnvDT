using EnvDT.Model.Core;
using Xunit;

namespace EnvDT.ModelTests.Core
{
    public class EvalCalcServiceTests
    {
        private EvalCalcService _evalCalcService;

        public EvalCalcServiceTests()
        {
            _evalCalcService = new EvalCalcService();
        }

        [Theory]
        [InlineData(1.0, "µg/l", "µg/l", 1.0)]
        [InlineData(1.0, "µg/l", "mg/l", 0.001)]
        [InlineData(1.0, "mg/l", "µg/l", 1000)]
        public void SampleValueConversionShouldReturnCorrectValue(
            double sampleValue, string sampleValueUnitName, string refValUnitName, double expectedValue)
        {
            var calculatedValue = _evalCalcService.SampleValueConversion(
                sampleValue, sampleValueUnitName, refValUnitName);

            Assert.Equal(calculatedValue, expectedValue);
    }
    }
}
