using EnvDT.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface IMissingParamDialogViewModel
    {
        public void Load(HashSet<Guid> ids);
        public ObservableCollection<MissingParamNameWrapper> MissingParamNames { get; }
    }
}
