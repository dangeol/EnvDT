using System;

namespace EnvDT.UI.Event
{
    public interface IDetailEventArgs
    {
        public Guid? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
