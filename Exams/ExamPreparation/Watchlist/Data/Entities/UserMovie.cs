using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Watchlist.Data.Entities
{
    public class UserMovie
    {
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = null!;

        public User User { get; set; } = null!;

        [Required]
        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        public Movie Movie { get; set; } = null!;
    }
}
