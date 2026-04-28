using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ToEmail).IsRequired().HasMaxLength(256);

            builder.Property(e => e.Subject).IsRequired().HasMaxLength(500);

            builder.Property(e => e.EventType).IsRequired().HasMaxLength(100);

            builder.Property(e => e.FailureReason).HasMaxLength(1000);

            builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasOne(e => e.Recipient).WithMany().HasForeignKey(e => e.RecipientUserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.SentAt);

            builder.HasIndex(e => e.IsSuccess);
        }
    }
}
