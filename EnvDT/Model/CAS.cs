using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class CAS
    {
        public int CASId { get; set; }
        public string CASNumber { get; set; }

        public int ParameterId { get; set; }
        public Parameter Parameter { get; set; }
    }
}
