using System.ComponentModel.DataAnnotations;
using Watchlist.Data.Entities;

namespace Watchlist.Models
{
    public class AddMovieViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Director { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), "0.0", "10.00")]
        public decimal Rating { get; set; }

        public int GenreId { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
