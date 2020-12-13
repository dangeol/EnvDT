using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class SampleWrapper : ModelWrapper<Sample>
    {
        private ObservableCollection<LookupItem> _media = new ObservableCollection<LookupItem>();
        private ObservableCollection<LookupItem> _mediumSubTypes = new ObservableCollection<LookupItem>();
        private ObservableCollection<LookupItem> _conditions = new ObservableCollection<LookupItem>();

        public SampleWrapper(Sample model) : base(model)
        {
        }

        public Guid SampleId
        {
            get { return Model.SampleId; }
        }

        public string SampleName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public Guid MediumId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<LookupItem> Media
        {
            get { return _media; }
            set
            {
                _media = value;
                OnPropertyChanged();
            }
        }

        public Guid MediumSubTypeId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<LookupItem> MediumSubTypes
        {
            get { return _mediumSubTypes; }
            set
            {
                _mediumSubTypes = value;
                OnPropertyChanged();
            }
        }

        public Guid ConditionId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<LookupItem> Conditions
        {
            get { return _conditions; }
            set
            {
                _conditions = value;
                OnPropertyChanged();
            }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(SampleName):
                    if (string.Equals(SampleName, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Sample Name cannot be empty.";
                    }
                    break;
                case nameof(MediumSubTypeId):
                    if (Guid.Equals(MediumSubTypeId, Guid.Empty))
                    {
                        yield return "Sub type must be chosen.";
                    }
                    break;
                case nameof(ConditionId):
                    if (Guid.Equals(ConditionId, Guid.Empty))
                    {
                        yield return "Condition must be chosen.";
                    }
                    break;
            }
        }
    }
}
