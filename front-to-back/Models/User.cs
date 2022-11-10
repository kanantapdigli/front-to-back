using Microsoft.AspNetCore.Identity;

namespace front_to_back.Models
{
    public class User : IdentityUser
    {
        public string Fullname { get; set; }
    }
}
