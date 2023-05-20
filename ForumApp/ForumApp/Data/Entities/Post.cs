using System.ComponentModel.DataAnnotations;
using static ForumApp.Data.DataConstants.Post;

namespace ForumApp.Data.Entities
{
	public class Post
	{
		[Key] // We can skip the [Key] attribute because ef core knows that it it a primary key because of the name conventions (it is called Id)
		public int Id { get; init; }

		[Required]
		[MaxLength(TitleMaxLength)]
		public string Title { get; set; } = null!;

		[Required]
		[MaxLength(ContentMaxLength)]
		public string Content { get; set; } = null!;
    }
}
