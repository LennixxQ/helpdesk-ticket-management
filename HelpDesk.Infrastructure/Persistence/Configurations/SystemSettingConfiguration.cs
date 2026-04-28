using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Key).IsRequired().HasMaxLength(100);

            builder.Property(s => s.Value).IsRequired().HasMaxLength(2000);

            builder.Property(s => s.Description).HasMaxLength(500);

            builder.HasOne(s => s.UpdatedBy).WithMany().HasForeignKey(s => s.UpdatedById).OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(s => s.Key).IsUnique();
        }
    }
}
