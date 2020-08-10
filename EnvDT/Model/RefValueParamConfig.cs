using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class RefValueParamConfig : IEntityTypeConfiguration<RefValueParam>
    {
        public void Configure(EntityTypeBuilder<RefValueParam> builder)
        {
            builder.HasKey(p => new { p.RefValueId, p.ParameterId });
            builder.HasOne(rp => rp.RefValue)
                .WithMany(r => r.RefValueParams)
                .HasForeignKey(rp => rp.RefValueId);
            builder.HasOne(rp => rp.Parameter)
                .WithMany(r => r.RefValueParams)
                .HasForeignKey(rp => rp.ParameterId);
        }
    }
}