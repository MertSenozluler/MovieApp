using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;


namespace MovieApp.Models.Domain
{
    public class ApplicationUser: IdentityUser
    {

        public string Name { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsActive { get; set; }

    }
}
