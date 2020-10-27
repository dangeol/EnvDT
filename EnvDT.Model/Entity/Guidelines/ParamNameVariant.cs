using System;

namespace EnvDT.Model.Entity
{
    public class ParamNameVariant
    {
        public Guid ParamNameVariantId { get; set; }
        public string ParamNameAlias { get; set; }

        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public Guid LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
