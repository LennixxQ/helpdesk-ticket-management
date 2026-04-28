using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class RecurringTemplateRunConfiguration : IEntityTypeConfiguration<RecurringTemplateRun>
    {
        public void Configure(EntityTypeBuilder<RecurringTemplateRun> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.FailureReason).HasMaxLength(500);

            builder.HasOne(r => r.GeneratedTicket).WithMany().HasForeignKey(r => r.GeneratedTicketId).OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(r => r.TemplateId);
        }
    }
}
