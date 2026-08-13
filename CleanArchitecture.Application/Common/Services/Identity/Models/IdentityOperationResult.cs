namespace CleanArchitecture.Application.Common.Services.Identity.Models
{
    public class IdentityOperationResult
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}
