using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Wrapper
{
    public class ConfigXmlWrapper : ModelWrapper<ConfigXml>
    {
        public ConfigXmlWrapper(ConfigXml model) : base(model)
        {
        }

        public Guid ConfigXmlId
        { 
            get { return Model.ConfigXmlId; } 
        }

        public string RootElement
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        // To be continued [...]

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(RootElement):
                    if (string.Equals(RootElement, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.ProjectWrapper_TextBlock_ProjectName"];
                    }
                    break;
            }
        }
    }
}
