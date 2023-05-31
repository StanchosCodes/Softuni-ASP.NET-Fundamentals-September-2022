using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Data.Models
{
    public class ApplicationUserBook
    {
        [Required]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; } = null!;

        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        public Book Book { get; set; } = null!;
    }
}
