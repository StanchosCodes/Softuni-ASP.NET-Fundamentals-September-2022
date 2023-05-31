using Library.Data;
using Library.Data.Models;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext context;

        public BooksController(LibraryDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> All()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Book> books = await this.context.Books
                .Include(b => b.Category)
                .ToListAsync();

            IEnumerable<BookViewModel> bookModels = books.Select(b => new BookViewModel()
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                Rating = b.Rating,
                Category = b.Category.Name
            });

            return View(bookModels);
        }

        public async Task<IActionResult> Mine()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await this.context.Users.Where(u => u.Id == userId)
                .Include(u => u.ApplicationUsersBooks)
                .ThenInclude(ub => ub.Book)
                .ThenInclude(b => b.Category)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user id!");
            }

            IEnumerable<BookViewModel> usersBooks = user.ApplicationUsersBooks
                .Select(ub => new BookViewModel()
                {
                    Id = ub.Book.Id,
                    Title = ub.Book.Title,
                    Author = ub.Book.Author,
                    Description = ub.Book.Description,
                    ImageUrl = ub.Book.ImageUrl,
                    Rating = ub.Book.Rating,
                    Category = ub.Book.Category.Name
                });

            return View("Mine", usersBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Category> categories = await this.context.Category.ToListAsync();

            AddBookViewModel bookModel = new AddBookViewModel()
            {
                Categories = categories
            };

            return View(bookModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookViewModel bookModel)
        {
            if (!ModelState.IsValid)
            {
                return View(bookModel);
            }

            try
            {
                Book newBook = new Book()
                {
                    Title = bookModel.Title,
                    Author = bookModel.Author,
                    Description = bookModel.Description,
                    ImageUrl = bookModel.ImageUrl,
                    Rating = bookModel.Rating,
                    CategoryId = bookModel.CategoryId
                };

                await this.context.Books.AddAsync(newBook);
                await this.context.SaveChangesAsync();

                return RedirectToAction("All", "Books");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to add book!");

                return View(bookModel);
            }
        }

        public async Task<IActionResult> AddToCollection(int bookId)
        {
            try
            {
                var userId = User!.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await this.context.Users
                    .Where(u => u.Id == userId)
                    .Include(u => u.ApplicationUsersBooks)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new ArgumentException("Invalid user id!");
                }

                var book = await this.context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

                if (book == null)
                {
                    throw new ArgumentException("Invalid book id!");
                }

                if (!user.ApplicationUsersBooks.Any(ub => ub.BookId == bookId))
                {
                    user.ApplicationUsersBooks.Add(new ApplicationUserBook()
                    {
                        ApplicationUserId = userId!,
                        BookId = bookId
                    });

                    await this.context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to add book!");
            }

            return RedirectToAction("All", "Books");
        }

        public async Task<IActionResult> RemoveFromCollection(int bookId)
        {
            var userId = User!.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await this.context.Users
                   .Where(u => u.Id == userId)
                   .Include(u => u.ApplicationUsersBooks)
                   .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user id!");
            }

            var book = user.ApplicationUsersBooks.FirstOrDefault(ub => ub.BookId == bookId);

            if (book != null)
            {
                user.ApplicationUsersBooks.Remove(book);
                await this.context.SaveChangesAsync();
            }

            return RedirectToAction("Mine", "Books");
        }
    }
}
