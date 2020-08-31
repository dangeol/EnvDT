using System;

namespace EnvDT.Model
{
    public class CAS
    {
        public Guid CASId { get; set; }
        public string CASNumber { get; set; }

        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
    }
}
