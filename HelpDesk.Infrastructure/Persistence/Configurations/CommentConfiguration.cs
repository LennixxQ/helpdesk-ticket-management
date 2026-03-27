using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        void IEntityTypeConfiguration<Comment>.Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Content).IsRequired().HasMaxLength(2000);
            builder.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.HasIndex(c => c.TicketId);
        }
    }
}
