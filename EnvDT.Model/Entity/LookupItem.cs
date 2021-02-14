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

    public class LookupItemConfigCsv
    {
        public Guid LookupItemId { get; set; }
        public string IdentWord { get; set; }
        public int IdentWordCol { get; set; }
        public int IdentWordRow { get; set; }
    }
}
