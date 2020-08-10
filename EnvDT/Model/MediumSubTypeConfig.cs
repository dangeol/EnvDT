using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model
{
    public class MediumSubTypeConfig : IEntityTypeConfiguration<MediumSubType>
    {
        public void Configure(EntityTypeBuilder<MediumSubType> builder)
        {
            builder.HasKey(m => m.MedSubTypeId);
        }
    }
}

