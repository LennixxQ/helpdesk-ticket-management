using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Title).IsRequired().HasMaxLength(150);
            
            builder.Property(t => t.Description).IsRequired().HasMaxLength(2000);
            
            builder.Property(t => t.Priority).IsRequired().HasConversion<string>().HasMaxLength(50);
            
            builder.Property(t => t.Status).IsRequired().HasConversion<string>().HasMaxLength(50).HasDefaultValue(TicketStatus.Open);
            
            builder.Property(t => t.SlaStatus).HasConversion<string>().HasMaxLength(50);
            
            builder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(256);
            
            builder.Property(t => t.LastModifiedBy).HasMaxLength(256);

            builder.HasOne(t => t.Category).WithMany(c => c.Tickets).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Comments).WithOne(c => c.Ticket).HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.SlaRecord).WithOne(s => s.Ticket).HasForeignKey<SlaRecord>(s => s.TicketId).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.EscalationRecord).WithOne(e => e.Ticket).HasForeignKey<EscalationRecord>(e => e.TicketId).OnDelete(DeleteBehavior.Cascade);

            
            builder.HasIndex(t => t.Status);
            
            builder.HasIndex(t => t.AssignedAgentId);
            
            builder.HasIndex(t => t.RaisedByUserId);
            
            builder.HasIndex(t => t.CreatedAt);
        }
    }
}
