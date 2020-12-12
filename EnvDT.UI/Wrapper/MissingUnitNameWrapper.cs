using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class MissingUnitNameWrapper : ModelWrapper<UnitNameVariant>
    {
        private ObservableCollection<string> _unitNameAliases = new ObservableCollection<string>();
        private string _unitName;

        public MissingUnitNameWrapper(UnitNameVariant model) : base(model)
        {
        }

        public Guid UnitNameVariantId
        { 
            get { return Model.UnitNameVariantId; } 
        }

        public string UnitName
        {
            get { return _unitName; }
            set
            {
                _unitName = value;
                OnPropertyChanged();
            }
        }

        public string UnitNameAlias
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<string> UnitNameAliases
        {
            get { return _unitNameAliases; }
            set
            {
                _unitNameAliases = value;
                OnPropertyChanged();
            }
        }

        public Guid UnitId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(UnitNameAlias):
                    if (string.Equals(UnitNameAlias, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Unit Name Alias must be chosen.";
                    }
                    break;
            }
        }
    }
}
