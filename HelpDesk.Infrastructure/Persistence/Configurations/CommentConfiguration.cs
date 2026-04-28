using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Content).IsRequired().HasMaxLength(2000);
            
            builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(256);
            
            builder.HasIndex(c => c.TicketId);
        }
    }
}
