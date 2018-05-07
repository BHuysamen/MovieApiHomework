using Microsoft.EntityFrameworkCore;
using Movies.Domain.Contracts;
using Movies.Domain.Entities;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Domain.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _context;
        
        public MovieRepository(MovieDbContext context)
        {
            _context = context;
            PopulateData();
        }

        public async Task<bool> DoesMovieExistAsync(int movieId)
        {
            return await _context.Movies.AnyAsync(m => m.Id == movieId);
        }

        public async Task<bool> DoesUserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(FilterModel filters)
        {
            var query = _context.Movies.AsQueryable();
            if (filters.Year > 0)
                query = query.Where(q => q.YearOfRelease == filters.Year);
            if (!string.IsNullOrEmpty(filters.Title))
                query = query.Where(q => q.Title.Contains(filters.Title));

            if (filters.Genres != null && filters.Genres.Count > 0)
            {
                var movieIds = await _context.MovieGenres.Where(m => filters.Genres.Contains(m.Genre.Name)).Select(a => a.MovieId).ToListAsync();
                query = query.Where(q => movieIds.Contains(q.Id));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByIdsAsync(List<int> movieIds)
        {
            return await _context.Movies.Where(m => movieIds.Contains(m.Id)).ToListAsync();
        }

        public async Task<IEnumerable<UserRating>> GetRatingsAsync(List<Movie> movies)
        {
            if (movies == null || movies.Count == 0)
                return new List<UserRating>();

            var movieIds = movies.Select(m => m.Id);

            return await _context.UserRatings.Where(u => movieIds.Contains(u.MovieId)).ToListAsync();
        }

        public async Task<IEnumerable<UserRating>> GetAllMovieRatingsAsync()
        {
            return await _context.UserRatings.ToListAsync();
        }

        public async Task<IEnumerable<UserRating>> GetAllMovieRatingsForUserAsync(int userId)
        {
            return await _context.UserRatings.Where(u => u.UserId == userId).ToListAsync();
        }

        // In memory, so not persisted between api calls.
        public async void UpsertMovieRatingForUser(int movieId, int userId, int rating)
        {
            var existingRating = await _context.UserRatings.Where(r => r.MovieId == movieId && r.UserId == userId).FirstOrDefaultAsync();

            if (existingRating == null)
            {
                _context.UserRatings.Add(new UserRating { MovieId = movieId, UserId = userId, Rating = rating });
            }
            else
            {
                existingRating.Rating = rating;
                _context.UserRatings.Update(existingRating);
            }

            await _context.SaveChangesAsync();
        }

        #region IDisposable

        private bool _disposed;

        ~MovieRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        #region Lets Avoid a Database...

        private void PopulateData()
        {
            _context.Movies.Add(new Movie { Id = 1, Title = "A Movie", YearOfRelease = 1998, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 2, Title = "AB Movie", YearOfRelease = 2001, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 3, Title = "C Movie", YearOfRelease = 2001, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 4, Title = "AD Movie", YearOfRelease = 2002, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 5, Title = "E Movie", YearOfRelease = 2007, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 6, Title = "F Movie", YearOfRelease = 2011, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 7, Title = "G Movie", YearOfRelease = 2014, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 8, Title = "H Movie", YearOfRelease = 2015, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 9, Title = "I Movie", YearOfRelease = 2017, RunningTime = 120 });
            _context.Movies.Add(new Movie { Id = 10, Title = "J Movie", YearOfRelease = 2018, RunningTime = 120 });

            _context.Genres.Add(new Genre { Id = 1, Name = "Comedy" });
            _context.Genres.Add(new Genre { Id = 2, Name = "Action" });
            _context.Genres.Add(new Genre { Id = 3, Name = "Thriller" });
            _context.Genres.Add(new Genre { Id = 4, Name = "Horror" });
            _context.Genres.Add(new Genre { Id = 5, Name = "Sci-Fi" });

            _context.MovieGenres.Add(new MovieGenre { MovieId = 1, GenreId = 1 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 2, GenreId = 4 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 3, GenreId = 4 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 4, GenreId = 4 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 5, GenreId = 5 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 6, GenreId = 1 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 7, GenreId = 2 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 8, GenreId = 3 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 9, GenreId = 4 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 10, GenreId = 5 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 1, GenreId = 2 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 2, GenreId = 3 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 3, GenreId = 5 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 4, GenreId = 3 });
            _context.MovieGenres.Add(new MovieGenre { MovieId = 5, GenreId = 1 });

            _context.UserRatings.Add(new UserRating { MovieId = 1, UserId = 1, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 2, UserId = 1, Rating = 3 });
            _context.UserRatings.Add(new UserRating { MovieId = 3, UserId = 1, Rating = 4 });
            _context.UserRatings.Add(new UserRating { MovieId = 4, UserId = 1, Rating = 3 });
            _context.UserRatings.Add(new UserRating { MovieId = 5, UserId = 1, Rating = 4 });
            _context.UserRatings.Add(new UserRating { MovieId = 6, UserId = 1, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 7, UserId = 1, Rating = 1 });
            _context.UserRatings.Add(new UserRating { MovieId = 8, UserId = 1, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 9, UserId = 1, Rating = 5 });

            _context.UserRatings.Add(new UserRating { MovieId = 4, UserId = 2, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 5, UserId = 2, Rating = 5 });
            _context.UserRatings.Add(new UserRating { MovieId = 6, UserId = 2, Rating = 3 });
            _context.UserRatings.Add(new UserRating { MovieId = 7, UserId = 2, Rating = 5 });
            _context.UserRatings.Add(new UserRating { MovieId = 8, UserId = 2, Rating = 3 });
            _context.UserRatings.Add(new UserRating { MovieId = 9, UserId = 2, Rating = 2 });

            _context.UserRatings.Add(new UserRating { MovieId = 1, UserId = 3, Rating = 5 });
            _context.UserRatings.Add(new UserRating { MovieId = 2, UserId = 3, Rating = 1 });
            _context.UserRatings.Add(new UserRating { MovieId = 3, UserId = 3, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 7, UserId = 3, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 8, UserId = 3, Rating = 5 });
            _context.UserRatings.Add(new UserRating { MovieId = 9, UserId = 3, Rating = 1 });

            _context.UserRatings.Add(new UserRating { MovieId = 1, UserId = 4, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 2, UserId = 4, Rating = 5 });
            _context.UserRatings.Add(new UserRating { MovieId = 3, UserId = 4, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 4, UserId = 4, Rating = 5 });
            _context.UserRatings.Add(new UserRating { MovieId = 5, UserId = 4, Rating = 2 });
            _context.UserRatings.Add(new UserRating { MovieId = 6, UserId = 4, Rating = 5 });

            _context.Users.Add(new User { Id = 1 });
            _context.Users.Add(new User { Id = 2 });
            _context.Users.Add(new User { Id = 3 });
            _context.Users.Add(new User { Id = 4 });

            _context.SaveChanges();
        }

        #endregion
    }
}

