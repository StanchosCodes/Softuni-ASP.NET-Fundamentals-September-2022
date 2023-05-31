using System.ComponentModel.DataAnnotations;

namespace Contacts.Data.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(60)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(13)]
        public string PhoneNumber { get; set; } = null!;
        // ^(0|(\+359))(\s|\-)?\d{3}(\3)?\d{2}(\3)?\d{2}(\3)?\d{2}$

        public string? Address { get; set; }

        [Required]
        public string Website { get; set; } = null!;
        // ^(www\.)(\w*|\d*|-*)+(\.bg)$

        public ICollection<ApplicationUserContact> ApplicationUsersContacts { get; set; } = new List<ApplicationUserContact>();
    }
}
