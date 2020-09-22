using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class DetailSavedEvent : PubSubEvent<DetailSavedEventArgs>
    {
    }

    public class DetailSavedEventArgs
    {
        public Guid Id { get; set; }
        public string DisplayMember { get; set; }
        public string ViewModelName { get; set; }
    }
}
