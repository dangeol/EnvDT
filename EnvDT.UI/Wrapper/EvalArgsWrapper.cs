using EnvDT.Model.Core.HelperClasses;
using EnvDT.Model.Entity;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class EvalArgsWrapper : ModelWrapper<EvalArgs>
    {
        private ObservableCollection<LookupItem> _mediumSubTypes = new ObservableCollection<LookupItem>();
        private ObservableCollection<LookupItem> _conditions = new ObservableCollection<LookupItem>();
        private LookupItem _selectedMediumSubType;
        private LookupItem _selectedCondition;

        public EvalArgsWrapper(EvalArgs model) : base(model)
        {
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

        public LookupItem SelectedMediumSubType
        {
            get { return _selectedMediumSubType; }
            set
            {
                _selectedMediumSubType = value;
                OnPropertyChanged();
            }
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

        public LookupItem SelectedCondition
        {
            get { return _selectedCondition; }
            set
            {
                _selectedCondition = value;
                OnPropertyChanged();
            }
        }
    }
}
