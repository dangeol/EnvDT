using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class LabReportParamConfig : IEntityTypeConfiguration<LabReportParam>
    {
        public void Configure(EntityTypeBuilder<LabReportParam> builder)
        {
            builder.HasKey(lp => lp.LabReportParamId);
            builder.HasOne(lr => lr.LabReport)
                .WithMany(lp => lp.LabReportParams)
                .HasForeignKey(lp => lp.LabReportId);
            builder.HasOne(lp => lp.Parameter)
                .WithMany(lr => lr.LabReportParams)
                .HasForeignKey(lp => lp.ParameterId);
            builder.HasOne(lp => lp.Unit)
                .WithMany(lr => lr.LabReportParams)
                .HasForeignKey(lp => lp.UnitId);
        }
    }
}