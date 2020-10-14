using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class DetailClosedEvent : PubSubEvent<DetailClosedEventArgs>
    {
    }

    public class DetailClosedEventArgs : IDetailEventArgs
    {
        public Guid? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
