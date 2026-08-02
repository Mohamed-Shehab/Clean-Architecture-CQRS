using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Table name
            builder.ToTable("Courses", table =>
            {
                table.HasCheckConstraint("CK_Courses_Capacity", "[Capacity] >= 0");
            });


            // Primary Key
            builder.HasKey(c => c.Id);


            // Properties
            builder.Property(c => c.NameEn)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(c => c.NameAr)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(c => c.Description)
                   .HasMaxLength(1000);

            builder.Property(c => c.Capacity)
                   .IsRequired();

            builder.Property(c => c.IsActive)
                   .HasDefaultValue(true);

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");


            builder.Property(x => x.RowVersion)
                   .IsRowVersion();


            // Query filters
            builder.HasQueryFilter(c => !c.IsDeleted);


            // Relationships
            builder.HasMany(c => c.Enrollments)
                   .WithOne(e => e.Course)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Indexes
            builder.HasIndex(c => c.NameEn)
                   .IsUnique();

            builder.HasIndex(c => c.NameAr)
                   .IsUnique();
        }
    }
}
