namespace Contacts.Data.Models
{
    public class ApplicationUserContact
    {
        public string ApplicationUserId { get; set; } = null!;

        public ApplicationUser ApplicationUser { get; set; } = null!;

        public int ContactId { get; set; }

        public Contact Contact { get; set; } = null!;
    }
}
