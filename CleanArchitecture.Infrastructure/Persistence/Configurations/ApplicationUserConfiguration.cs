using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Table name
            builder.ToTable("Users");


            // Properties
            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");


            // Query Filter
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
