using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class CsatResponseConfiguration : IEntityTypeConfiguration<CsatResponse>
    {
        public void Configure(EntityTypeBuilder<CsatResponse> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Score).IsRequired();

            builder.Property(c => c.Comments).HasMaxLength(500);

            builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasOne(c => c.Ticket).WithMany().HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Respondent).WithMany().HasForeignKey(c => c.RespondentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ClosingAgent).WithMany().HasForeignKey(c => c.ClosingAgentId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.TicketId).IsUnique();

            builder.HasIndex(c => c.ClosingAgentId);
        }
    }
}
