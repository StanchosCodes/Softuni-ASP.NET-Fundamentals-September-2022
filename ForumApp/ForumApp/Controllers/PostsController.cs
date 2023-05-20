using ForumApp.Data;
using ForumApp.Data.Entities;
using ForumApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ForumApp.Controllers
{
	public class PostsController : Controller
	{
		private readonly ForumAppDbContext context;

        public PostsController(ForumAppDbContext context)
        {
			this.context = context;
        }

		public IActionResult All()
		{
			var posts = this.context.Posts
				.Select(p => new PostViewModel()
				{
					Id = p.Id,
					Title = p.Title,
					Content = p.Content
				})
				.ToList();

			return View(posts);
		}

		[HttpGet]
		public IActionResult Add() // This method returns the empty view when add post btn is hit so we can see the page
		{
			return View();
		}

		[HttpPost]
		public IActionResult Add(PostFormViewModel model) // Then this method adds the post when creat btn is hit
		{
			var post = new Post()
			{
				Title = model.Title,
				Content = model.Content
			};

			this.context.Posts.Add(post);
			this.context.SaveChanges();

			return RedirectToAction("All");
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			Post? post = this.context.Posts.Find(id);

			PostFormViewModel postToEdit = new PostFormViewModel()
			{
				Title = post.Title,
				Content = post.Content
			};

			return View(postToEdit);
		}

		[HttpPost]
		public IActionResult Edit(int id, PostFormViewModel editedPost)
		{
			Post? post = this.context.Posts.Find(id);

			post.Title = editedPost.Title;
			post.Content = editedPost.Content;

			this.context.SaveChanges();

			return RedirectToAction("All");
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			Post? postToDelete = this.context.Posts.Find(id);

			this.context.Posts.Remove(postToDelete);
			this.context.SaveChanges();

			return RedirectToAction("All");
		}
	}
}
