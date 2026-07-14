using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Table name
            builder.ToTable("Enrollments", table =>
            {
                table.HasCheckConstraint("CK_Enrollments_Status", "[Status] IN (1,2,3)");
            });


            // Composite Key
            builder.HasKey(e => new { e.StudentId, e.CourseId });


            // Properties
            builder.Property(e => e.EnrolledAt)
                   .IsRequired();

            builder.Property(e => e.Status)
                   .HasDefaultValue(EnrollmentStatus.Active);


            // Relationships
            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Indexes
            builder.HasIndex(e => e.Status);
        }
    }
}
