using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class RefValueNameConfig : IEntityTypeConfiguration<RefValueName>
    {
        public void Configure(EntityTypeBuilder<RefValueName> builder)
        {
            builder.HasKey(r => r.RefValueNameId);
        }
    }
}
