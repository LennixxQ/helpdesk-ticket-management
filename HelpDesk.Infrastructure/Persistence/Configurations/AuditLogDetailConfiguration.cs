using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class AuditLogDetailConfiguration : IEntityTypeConfiguration<AuditLogDetail>
    {
        void IEntityTypeConfiguration<AuditLogDetail>.Configure(EntityTypeBuilder<AuditLogDetail> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FieldName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.OldValue).HasMaxLength(2000);
            builder.Property(d => d.NewValue).HasMaxLength(2000);
        }
    }
}
