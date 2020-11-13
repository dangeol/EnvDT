using System;
using System.Collections.Generic;

namespace EnvDT.UI.ViewModel
{
    public interface IDetailViewModel
    {
        void Load(Guid? id);
        public bool HasChanges { get; }
    }
}
