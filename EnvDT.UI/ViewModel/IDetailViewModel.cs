using System;

namespace EnvDT.UI.ViewModel
{
    public interface IDetailViewModel
    {
        void Load(Guid? id);
        public bool HasChanges { get; }
    }
}
