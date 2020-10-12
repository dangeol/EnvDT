using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class DetailClosedEvent : PubSubEvent<DetailClosedEventArgs>
    {
    }

    public class DetailClosedEventArgs
    {
        public Guid? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
