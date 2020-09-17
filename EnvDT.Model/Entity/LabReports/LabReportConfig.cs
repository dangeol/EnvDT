using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnvDT.Model.Entity
{
    public class LabReportConfig : IEntityTypeConfiguration<LabReport>
    {
        public void Configure(EntityTypeBuilder<LabReport> builder)
        {
            builder.HasKey(l => l.LabReportId);
            builder.HasOne(lp => lp.Project)
                .WithMany(p => p.LabReports)
                .HasForeignKey(lp => lp.ProjectId);
            builder.HasOne(ll => ll.Laboratory)
                .WithMany(l => l.LabReports)
                .HasForeignKey(ll => ll.LaboratoryId);
        }
    }
}