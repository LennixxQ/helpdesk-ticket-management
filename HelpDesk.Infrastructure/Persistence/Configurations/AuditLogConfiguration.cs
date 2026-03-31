using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        void IEntityTypeConfiguration<AuditLog>.Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
            builder.Property(a => a.IpAddress).HasMaxLength(45);
            builder.Property(a => a.PerformedBy).IsRequired().HasMaxLength(256);
            builder.HasMany(a => a.Details).WithOne(d => d.AuditLog).HasForeignKey(d => d.AuditLogId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
