using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        void IEntityTypeConfiguration<AuditLog>.Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
            builder.Property(a => a.IPAddress).HasMaxLength(45);
            builder.Property(a => a.PerformedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.HasMany(a => a.Details).WithOne(d => d.AuditLog).HasForeignKey(d => d.AuditLogId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(a => a.EntityId);
            builder.HasIndex(a => a.PerformedByUserId);
            builder.HasIndex(a => a.PerformedAt);
        }
    }
}
