using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class EscalationRecordConfiguration : IEntityTypeConfiguration<EscalationRecord>
    {
        public void Configure(EntityTypeBuilder<EscalationRecord> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Reason).IsRequired().HasMaxLength(500);
            
            builder.Property(e => e.Trigger).IsRequired().HasConversion<string>().HasMaxLength(50);
            
            builder.Property(e => e.EscalatedBy).IsRequired().HasMaxLength(256);
            
            builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasOne(e => e.EscalatedByUser).WithMany().HasForeignKey(e => e.EscalatedByUserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.AcknowledgedByUser).WithMany().HasForeignKey(e => e.AcknowledgedByUserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.TicketId).IsUnique();
        }
    }
}
