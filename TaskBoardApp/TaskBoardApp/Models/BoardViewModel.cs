namespace TaskBoardApp.Models
{
    public class BoardViewModel
	{
		public int Id { get; init; }

		public string Name { get; init; } = null!;

		public ICollection<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
    }
}
