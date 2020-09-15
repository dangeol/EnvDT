using EnvDT.Model;
using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class ProjectDeletedEvent : PubSubEvent<Guid>
    {
    }
}
