using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Watchlist.Data;
using Watchlist.Data.Entities;
using Watchlist.Models;

namespace Watchlist.Controllers
{
    public class MoviesController : Controller
    {
        private readonly WatchlistDbContext context;

        public MoviesController(WatchlistDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Movie> entities = await this.context.Movies
                .Include(m => m.Genre)
                .ToListAsync();

            IEnumerable<MovieViewModel> model = entities.Select(e => new MovieViewModel()
            {
                Id = e.Id,
                Title = e.Title,
                Director = e.Director,
                ImageUrl = e.ImageUrl,
                Rating = e.Rating,
                Genre = e.Genre.Name
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Genre> genres = await this.context.Genres.ToListAsync();

            AddMovieViewModel movieModel = new AddMovieViewModel()
            {
                Genres = genres
            };

            return View(movieModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMovieViewModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return View(movieModel);
            }

            try
            {
                Movie newMovie = new Movie()
                {
                    Title = movieModel.Title,
                    Director = movieModel.Director,
                    ImageUrl = movieModel.ImageUrl,
                    Rating = movieModel.Rating,
                    GenreId = movieModel.GenreId
                };

                await this.context.Movies.AddAsync(newMovie);
                await this.context.SaveChangesAsync();

                return RedirectToAction("All", "Movies");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to add movie");

                return View(movieModel);
            }
        }

        public async Task<IActionResult> AddToCollection(int movieId)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await this.context.Users
                    .Where(u => u.Id == userId)
                    .Include(u => u.UsersMovies)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new ArgumentException("Invaid user id!");
                }

                var movie = await this.context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

                if (movie == null)
                {
                    throw new ArgumentException("Invaid movie id!");
                }

                if (!user.UsersMovies.Any(m => m.MovieId == movieId))
                {
                    user.UsersMovies.Add(new UserMovie()
                    {
                        UserId = userId!,
                        MovieId = movieId
                    });

                    await this.context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to add movie");
            }

            return RedirectToAction("All", "Movies");
        }

        public async Task<IActionResult> Watched()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // string

            var user = await this.context.Users.Where(u => u.Id == userId) // Entities.User
                .Include(u => u.UsersMovies)
                .ThenInclude(um => um.Movie)
                .ThenInclude(um => um.Genre)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invaid user id!");
            }

            IEnumerable<MovieViewModel> usersMovies = user.UsersMovies
                .Select(m => new MovieViewModel()
                {
                    Id = m.Movie.Id,
                    Title = m.Movie.Title,
                    Director = m.Movie.Director,
                    ImageUrl = m.Movie.ImageUrl,
                    Rating = m.Movie.Rating,
                    Genre = m.Movie.Genre?.Name
                });

            return View("Watched", usersMovies);
        }

        public async Task<IActionResult> RemoveFromCollection(int movieId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; // string

            var user = await this.context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UsersMovies)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invaid user id!");
            }

            var movie = user.UsersMovies.FirstOrDefault(m => m.MovieId == movieId); // Entities.UserMovie

            if (movie != null)
            {
                user.UsersMovies.Remove(movie);
                await this.context.SaveChangesAsync();
            }

            return RedirectToAction("Watched", "Movies");
        }
    }
}
