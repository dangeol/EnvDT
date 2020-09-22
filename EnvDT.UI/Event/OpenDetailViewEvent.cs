using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class OpenDetailViewEvent : PubSubEvent<OpenDetailViewEventArgs>
    {
    }
    public class OpenDetailViewEventArgs
    {
        public Guid? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
