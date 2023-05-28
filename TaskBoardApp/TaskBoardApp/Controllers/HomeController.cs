using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TaskBoardApp.Data;
using TaskBoardApp.Models;

namespace TaskBoardApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly TaskBoardAppDbContext context;

        public HomeController(TaskBoardAppDbContext context)
        {
			this.context = context;
        }

        public IActionResult Index()
		{
			var taskBoards = this.context.Boards
				.Select(b => b.Name)
				.Distinct()
				.ToList(); // if we remove .ToList() the data reader stays open to the database and when we encounter another request to the database later on it will throw argument exception with There is already an open DataReader message

			List<HomeBoardModel> tasksCounts = new List<HomeBoardModel>();

			foreach (string boardName in taskBoards)
			{
				var tasksInBoard = this.context.Tasks.Where(t => t.Board.Name == boardName).Count(); // second request to the database

				tasksCounts.Add(new HomeBoardModel()
				{
					BoardName = boardName,
					TasksCount = tasksInBoard
				});
			}

			int userTasksCount = -1;

			if (this.User?.Identity?.IsAuthenticated ?? false)
			{
				string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				userTasksCount = this.context.Tasks.Where(t => t.OwnerId == currentUserId).Count();
			}

			var homeModel = new HomeViewModel()
			{
				AllTasksCount = this.context.Tasks.Count(),
				BoardsWithTasksCount = tasksCounts,
				UserTasksCount = userTasksCount
			};

			return View(homeModel);
		}
	}
}