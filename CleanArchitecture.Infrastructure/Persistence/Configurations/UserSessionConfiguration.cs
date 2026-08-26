using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            // Table name
            builder.ToTable("UserSessions");


            // Primary Key
            builder.HasKey(x => x.UserSessionId);


            // Foreign Key
            builder.Property(x => x.UserId)
                .IsRequired();


            // Properties
            builder.Property(x => x.RefreshTokenHash)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(x => x.RefreshTokenExpiresAt)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.LastUsedAt)
                   .IsRequired(false);

            builder.Property(x => x.RevokedAt)
                   .IsRequired(false);


            builder.Property(x => x.IpAddress)
                   .HasMaxLength(45)
                   .IsRequired(false);

            builder.Property(x => x.UserAgent)
                   .HasMaxLength(2048)
                   .IsRequired(false);


            // Relationship
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Indexes
            builder.HasIndex(x => x.RefreshTokenHash)
                   .IsUnique();

            builder.HasIndex(x => x.UserId);
        }
    }
}
