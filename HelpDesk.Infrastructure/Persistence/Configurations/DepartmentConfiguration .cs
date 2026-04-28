using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);
            
            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            
            builder.Property(d => d.CreatedBy).IsRequired().HasMaxLength(256);
            
            builder.Property(d => d.LastModifiedBy).HasMaxLength(256);

            builder.HasOne(d => d.DepartmentHead).WithMany().HasForeignKey(d => d.DepartmentHeadId).OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(d => d.Members).WithOne(u => u.Department).HasForeignKey(u => u.DepartmentId).OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(d => d.Tickets).WithOne(t => t.Department).HasForeignKey(t => t.DepartmentId).OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(d => d.Name).IsUnique();

        }
    }
}
