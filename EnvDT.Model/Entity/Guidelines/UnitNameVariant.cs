using System;

namespace EnvDT.Model.Entity
{
    public class UnitNameVariant
    {
        public Guid UnitNameVariantId { get; set; }
        public string UnitNameAlias { get; set; }

        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
