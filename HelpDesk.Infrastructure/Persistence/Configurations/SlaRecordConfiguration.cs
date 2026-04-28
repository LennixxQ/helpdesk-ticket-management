using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class SlaRecordConfiguration : IEntityTypeConfiguration<SlaRecord>
    {
        public void Configure(EntityTypeBuilder<SlaRecord> builder)
        {
            
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(50);
            
            builder.Property(s => s.OverrideReason).HasMaxLength(500);
            
            builder.Property(s => s.CreatedBy).IsRequired().HasMaxLength(256);
            
            builder.HasIndex(s => s.TicketId).IsUnique();
            
            builder.HasIndex(s => s.SlaDeadline);
        }
    }
}
