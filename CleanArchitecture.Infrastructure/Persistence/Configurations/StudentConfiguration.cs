using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Table name
            builder.ToTable("Students");


            // Primary key
            builder.HasKey(s => s.Id);
            

            // Properties
            builder.Property(s => s.DateOfBirth)
                   .IsRequired();

            builder.Property(s => s.Address)
                   .HasMaxLength(250);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);


            builder.Property(x => x.RowVersion)
                   .IsRowVersion();


            // Query filters
            builder.HasQueryFilter(s => !s.IsDeleted);


            // Relationships
            builder.HasOne<ApplicationUser>()
                   .WithOne()
                   .HasForeignKey<Student>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Enrollments)
                   .WithOne(e => e.Student)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Indexes
            builder.HasIndex(s => s.UserId)
                   .IsUnique();
        }
    }
}
