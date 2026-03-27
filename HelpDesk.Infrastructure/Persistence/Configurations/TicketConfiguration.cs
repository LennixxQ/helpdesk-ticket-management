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
            builder.Property(t => t.Description).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Priority).IsRequired().HasConversion<string>().HasMaxLength(50);
            builder.Property(t => t.Status).IsRequired().HasConversion<string>().HasMaxLength(50).HasDefaultValue(TicketStatus.Open);
            builder.Property(t => t.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(t => t.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne(t => t.Category).WithMany(c => c.Tickets).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(t => t.Comments).WithOne(c => c.Ticket).HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.AssignedAgentId);
            builder.HasIndex(t => t.RaisedByUserId);
            builder.HasIndex(t => t.CreatedAt);
        }
    }
}
