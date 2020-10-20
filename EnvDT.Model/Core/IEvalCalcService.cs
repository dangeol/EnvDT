using EnvDT.Model.Core.HelperClasses;
using System;

namespace EnvDT.Model.Core
{
    public interface IEvalCalcService
    {
        public double SampleValueConversion(double sampleValue, string sampleValueUnitName, string refValUnitName);
    }
}