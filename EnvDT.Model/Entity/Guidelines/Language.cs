using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Language
    {
        public Guid LanguageId { get; set; }
        public string LangAbbrev { get; set; }
        public string LangName { get; set; }

        public List<ParamNameVariant> ParamNameVariants { get; set; }
    }
}
