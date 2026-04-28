using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class KbArticleConfiguration : IEntityTypeConfiguration<KbArticle>
    {
        public void Configure(EntityTypeBuilder<KbArticle> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.Title).IsRequired().HasMaxLength(300);

            builder.Property(k => k.Content).IsRequired();

            builder.Property(k => k.Tags).HasMaxLength(500);

            builder.Property(k => k.Status).IsRequired().HasConversion<string>().HasMaxLength(50);

            builder.Property(k => k.CreatedBy).IsRequired().HasMaxLength(256);

            builder.HasOne(k => k.Category).WithMany().HasForeignKey(k => k.CategoryId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(k => k.Author).WithMany().HasForeignKey(k => k.AuthorId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(k => k.LastUpdatedBy).WithMany().HasForeignKey(k => k.LastUpdatedById).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(k => k.Versions).WithOne(v => v.KbArticle).HasForeignKey(v => v.KbArticleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
