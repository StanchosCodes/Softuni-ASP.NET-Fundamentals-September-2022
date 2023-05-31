using System.ComponentModel.DataAnnotations;

namespace Contacts.Models
{
    public class AddContactViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(60, MinimumLength = 10)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(13, MinimumLength = 10)]
        [RegularExpression(@"^(0|(\+359))(\s|\-)?\d{3}(\3)?\d{2}(\3)?\d{2}(\3)?\d{2}$")]
        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; }

        [Required]
        [RegularExpression(@"^(www\.)(\w*|\d*|-*)+(\.bg)$")]
        public string Website { get; set; } = null!;
    }
}
