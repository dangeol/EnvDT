using EnvDT.UI.Wrapper;
using System;

namespace EnvDT.UI.ViewModel
{
    public interface IEditViewModel
    {
        void Load(Guid? id);
        //ModelWrapper<object> Model { get; }
        public bool IsChanged { get; }
    }
}
