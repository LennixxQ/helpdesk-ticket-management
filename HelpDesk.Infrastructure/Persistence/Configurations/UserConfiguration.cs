using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(50);
            builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(u => u.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.HasMany(u => u.RaisedTickets).WithOne(t => t.RaisedByUser).HasForeignKey(t => t.RaisedByUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.AssignedTickets).WithOne(t => t.AssignedAgent).HasForeignKey(t => t.AssignedAgentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Comments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.AuditLogs).WithOne(a => a.PerformedByUser).HasForeignKey(a => a.PerformedByUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
