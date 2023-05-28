using Microsoft.AspNetCore.Mvc;
using TaskBoardApp.Data;
using TaskBoardApp.Models;
//using TaskBoardApp.Models.Tasks;

namespace TaskBoardApp.Controllers
{
	public class BoardsController : Controller
	{
		private readonly TaskBoardAppDbContext context;

        public BoardsController(TaskBoardAppDbContext context)
        {
			this.context = context;
        }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult All()
		{
			List<BoardViewModel> boards = this.context.Boards
				.Select(b => new BoardViewModel()
				{
					Id = b.Id,
					Name = b.Name,
					Tasks = b.Tasks.Select(t => new TaskViewModel()
					{
						Id = t.Id,
						Title = t.Title,
						Description = t.Description,
						Owner = t.Owner.UserName
					})
					.ToList()
				})
				.ToList();

			return View(boards);
		}
	}
}
