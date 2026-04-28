using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class KbArticleVersionConfiguration : IEntityTypeConfiguration<KbArticleVersion>
    {
        public void Configure(EntityTypeBuilder<KbArticleVersion> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Title).IsRequired().HasMaxLength(300);

            builder.Property(v => v.Content).IsRequired();

            builder.HasOne(v => v.SavedByUser).WithMany().HasForeignKey(v => v.SavedByUserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(v => v.KbArticleId);
        }
    }
}
