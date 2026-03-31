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
            builder.Property(t => t.Priority).HasConversion<string>().IsRequired();
            builder.Property(t => t.Status).HasConversion<string>().IsRequired();
            builder.HasOne(t => t.Category).WithMany(c => c.Tickets).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(t => t.RaisedByUser).WithMany(u => u.RaisedTickets).HasForeignKey(t => t.RaisedByUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(t => t.AssignedAgent).WithMany(u => u.AssignedTickets).HasForeignKey(t => t.AssignedAgentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(t => t.Comments).WithOne(c => c.Ticket).HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
