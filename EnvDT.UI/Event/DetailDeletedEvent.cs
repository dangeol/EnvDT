using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class DetailDeletedEvent : PubSubEvent<DetailDeletedEventArgs>
    {
    }

    public class DetailDeletedEventArgs
    {
        public Guid? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
