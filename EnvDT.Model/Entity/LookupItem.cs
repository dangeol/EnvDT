using System;

namespace EnvDT.Model.Entity
{
    public class LookupItem
    {
        public Guid LookupItemId { get; set; }
        public string DisplayMember { get; set; }
    }

    public class LookupItemNull : LookupItem
    {
        public new Guid LookupItemId { get { return Guid.Empty; } }
        public new string DisplayMember { get { return " - "; } }
    }
}
