using System;

namespace EnvDT.Model.Entity
{
    public class ParameterLaboratory
    {
        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }

        public Guid LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; }

        public string LabParamName { get; set; }
    }
}
