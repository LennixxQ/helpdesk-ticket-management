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
    public class AuditLogDetailConfiguration : IEntityTypeConfiguration<AuditLogDetail>
    {
        void IEntityTypeConfiguration<AuditLogDetail>.Configure(EntityTypeBuilder<AuditLogDetail> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FieldName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.OldValue).HasMaxLength(200);
            builder.Property(d => d.NewValue).HasMaxLength(200);
        }
    }
}
