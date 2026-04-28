using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);

        builder.Property(u => u.CreatedBy).IsRequired().HasMaxLength(256);

        builder.Property(u => u.LastModifiedBy).HasMaxLength(256);

        builder.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(50);

        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasMany(u => u.RaisedTickets).WithOne(t => t.RaisedByUser).HasForeignKey(t => t.RaisedByUserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AssignedTickets).WithOne(t => t.AssignedAgent).HasForeignKey(t => t.AssignedAgentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Comments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(u => u.AuditLogs);
    }
}