using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskBoardApp.Data.Entities;

namespace TaskBoardApp.Data
{
	public class TaskBoardAppDbContext : IdentityDbContext<User>
	{
		private User GuestUser { get; set; } = null!;
		private Board OpenBoard { get; set; } = null!;
		private Board InProgressBoard { get; set; } = null!;
		private Board DoneBoard { get; set; } = null!;

		public TaskBoardAppDbContext(DbContextOptions<TaskBoardAppDbContext> options)
			: base(options)
		{
			this.Database.Migrate();
		}

		public DbSet<Board> Boards { get; set; } = null!;
		public DbSet<Entities.Task> Tasks { get; set; } = null!;

		private void SeedUsers()
		{
			PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();

			this.GuestUser = new User()
			{
				UserName = "Guest",
				NormalizedUserName = "GUEST",
				Email = "Guest@abv.bg",
				NormalizedEmail = "GUEST@ABV.BG",
				FirstName = "Guest",
				LastName = "User"
			};

			this.GuestUser.PasswordHash = hasher.HashPassword(this.GuestUser, "guest");
		}

		private void SeedBoards()
		{
			this.OpenBoard = new Board()
			{
				Id = 1,
				Name = "Open"
			};

			this.InProgressBoard = new Board()
			{
				Id = 2,
				Name = "In Progress"
			};

			this.DoneBoard = new Board()
			{
				Id = 3,
				Name = "Done"
			};
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			SeedUsers();

			builder
				.Entity<User>()
				.HasData(this.GuestUser);

			SeedBoards();

			builder
				.Entity<Board>()
				.HasData(this.OpenBoard, this.InProgressBoard, this.DoneBoard);

			builder
				.Entity<Entities.Task>()
				.HasOne(t => t.Board)
				.WithMany(b => b.Tasks)
				.HasForeignKey(t => t.BoardId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.Entity<Entities.Task>()
				.HasData(new Entities.Task()
				{
					Id = 1,
					Title = "Prepare for ASP.NET Fundamentals exam",
					Description = "Leard using ASP.NET Core Identity",
					CreatedOn = DateTime.Now,
					BoardId = this.OpenBoard.Id,
					OwnerId = this.GuestUser.Id
				},
				new Entities.Task()
				{
					Id = 2,
					Title = "Improve EF Core skills",
					Description = "Leard using EF Core and MS SQL Server Managment Studio",
					CreatedOn = DateTime.Now.AddMonths(-2),
					BoardId = this.DoneBoard.Id,
					OwnerId = this.GuestUser.Id
				},
				new Entities.Task()
				{
					Id = 3,
					Title = "Improve ASP.NET Core skills",
					Description = "Leard using ASP.NET Core Identity",
					CreatedOn = DateTime.Now.AddDays(-10),
					BoardId = this.InProgressBoard.Id,
					OwnerId = this.GuestUser.Id
				},
				new Entities.Task()
				{
					Id = 4,
					Title = "Prepare for C# Fundamentals exam",
					Description = "Prepare by solving old Mid and Final exams",
					CreatedOn = DateTime.Now.AddYears(-1),
					BoardId = this.DoneBoard.Id,
					OwnerId = this.GuestUser.Id
				});

			base.OnModelCreating(builder);
		}
	}
}