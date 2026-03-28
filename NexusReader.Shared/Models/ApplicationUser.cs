using Microsoft.AspNetCore.Identity;

namespace NexusReader.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsOver18 { get; set; }
    }
}
