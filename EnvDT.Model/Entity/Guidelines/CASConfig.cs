using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class CASConfig : IEntityTypeConfiguration<CAS>
    {
        public void Configure(EntityTypeBuilder<CAS> builder)
        {
            builder.HasKey(c => c.CASId);
            builder.HasOne(c => c.Parameter)
                .WithMany(c => c.CASs)
                .HasForeignKey(c => c.ParameterId);
        }
    }
}