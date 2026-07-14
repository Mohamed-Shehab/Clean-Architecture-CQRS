using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
