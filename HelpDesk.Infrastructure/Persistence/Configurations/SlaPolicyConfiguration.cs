using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class SlaPolicyConfiguration : IEntityTypeConfiguration<SlaPolicy>
    {
        public void Configure(EntityTypeBuilder<SlaPolicy> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Priority).IsRequired().HasConversion<string>().HasMaxLength(50);

            builder.Property(s => s.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasIndex(s => s.Priority).IsUnique();
        }
    }
}
