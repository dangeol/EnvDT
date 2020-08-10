using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class MediumConfig : IEntityTypeConfiguration<Medium>
    {
        public void Configure(EntityTypeBuilder<Medium> builder)
        {
            builder.HasKey(m => m.MediumId);
        }
    }
}
