using Prism.Events;
using System;

namespace EnvDT.UI.Event
{
    public class LabReportImportedEvent : PubSubEvent<LabReportImportedEventArgs>
    {
    }

    public class LabReportImportedEventArgs
    {
        public Guid Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
