using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class AfterPublicationSavedEvent : PubSubEvent<AfterPublicationSavedEventArgs>
    {
    }

    public class AfterPublicationSavedEventArgs
    {
        public Guid PublicationId { get; set; }
        public string DisplayMember { get; set; }
    }
}
