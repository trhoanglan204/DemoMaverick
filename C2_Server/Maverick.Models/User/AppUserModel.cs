using Microsoft.AspNetCore.Identity;

namespace Maverick.Models.User
{
    public class AppUserModel : IdentityUser
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
    }
}
