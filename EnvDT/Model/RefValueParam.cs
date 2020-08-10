using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class RefValueParam
    {
        public int RefValueId { get; set; }
        public RefValue RefValue { get; set; }

        public int ParameterId { get; set; }
        public Parameter Parameter { get; set; }
    }
}
