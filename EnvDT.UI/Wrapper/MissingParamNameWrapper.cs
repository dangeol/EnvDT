using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Wrapper
{
    public class MissingParamNameWrapper : ModelWrapper<ParamNameVariant>
    {
        public MissingParamNameWrapper(ParamNameVariant model) : base(model)
        {
        }

        public Guid ParamNameVariantId
        { 
            get { return Model.ParamNameVariantId; } 
        }

        public string ParamNameAlias
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public Guid ParameterId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public Guid LanguageId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(ParamNameAlias):
                    if (string.Equals(ParamNameAlias, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Parameter Name Alias cannot be empty.";
                    }
                    if (Guid.Equals(ParameterId, Guid.Empty))
                    {
                        yield return "ParameterId cannot be empty.";
                    }
                    if (Guid.Equals(LanguageId, Guid.Empty))
                    {
                        yield return "LanguageId Name cannot be empty.";
                    }
                    break;
            }
        }
    }
}
