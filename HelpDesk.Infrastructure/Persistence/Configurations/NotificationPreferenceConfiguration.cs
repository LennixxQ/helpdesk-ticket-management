using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.HasKey(n => n.Id);
            
            builder.Property(n => n.EventType).IsRequired().HasConversion<string>().HasMaxLength(100);
            
            builder.Property(n => n.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasOne(n => n.User).WithMany(u => u.NotificationPreferences).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => new { n.UserId, n.EventType }).IsUnique();
        }
    }
}
