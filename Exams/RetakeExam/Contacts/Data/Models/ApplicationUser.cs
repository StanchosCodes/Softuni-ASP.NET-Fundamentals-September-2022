using Microsoft.AspNetCore.Identity;

namespace Contacts.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ApplicationUserContact> ApplicationUsersContacts { get; set; } = new List<ApplicationUserContact>();
    }
}
