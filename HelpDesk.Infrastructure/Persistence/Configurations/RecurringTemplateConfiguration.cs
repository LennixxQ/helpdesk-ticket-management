using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class RecurringTemplateConfiguration : IEntityTypeConfiguration<RecurringTemplate>
    {
        public void Configure(EntityTypeBuilder<RecurringTemplate> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.TemplateName).IsRequired().HasMaxLength(200);

            builder.Property(r => r.TicketTitle).IsRequired().HasMaxLength(150);

            builder.Property(r => r.Description).IsRequired().HasMaxLength(2000);

            builder.Property(r => r.Priority).IsRequired().HasConversion<string>().HasMaxLength(50);

            builder.Property(r => r.RecurrencePattern).IsRequired().HasConversion<string>().HasMaxLength(50);

            builder.Property(r => r.CronExpression).HasMaxLength(100);

            builder.Property(r => r.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasOne(r => r.Category).WithMany().HasForeignKey(r => r.CategoryId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.CreatedByAdmin).WithMany().HasForeignKey(r => r.CreatedByAdminId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.AssignToAgent).WithMany().HasForeignKey(r => r.AssignToAgentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RaiseOnBehalfOf).WithMany().HasForeignKey(r => r.RaiseOnBehalfOfId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Runs).WithOne(run => run.Template).HasForeignKey(run => run.TemplateId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.IsActive);

            builder.HasIndex(r => r.NextRunAt);
        }
    }
}
