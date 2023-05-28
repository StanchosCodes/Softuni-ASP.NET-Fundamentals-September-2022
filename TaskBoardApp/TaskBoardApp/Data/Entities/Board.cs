using System.ComponentModel.DataAnnotations;
using static TaskBoardApp.Data.DataConstants.Board;

namespace TaskBoardApp.Data.Entities
{
	public class Board
	{
        public int Id { get; init; }

        [Required]
        [MaxLength(BoardNameMaxLength)]
        public string Name { get; init; } = null!;

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
