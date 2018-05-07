using Movies.Domain.Contracts;
using Movies.Domain.Entities;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public bool ValidateMovieFilters(FilterModel filters)
        {
            if (filters == null || filters.Year < 0)
                return false;

            if (filters.Genres != null)
            {
                foreach (var genre in filters.Genres)
                {
                    if (string.IsNullOrEmpty(genre))
                        return false;
                }

                if (filters.Genres.Count() > 0)
                    return true;
            }

            return filters.Year > 0 || !string.IsNullOrEmpty(filters.Title);
        }

        public bool ValidateMovieRating(int rating)
        {
            return rating >= 0 && rating <= 5;
        }

        public async Task<bool> ValidateMovieIdAsync(int movieId)
        {
            return await _movieRepository.DoesMovieExistAsync(movieId);
        }

        public async Task<bool> ValidateUserIdAsync(int userId)
        {
            return await _movieRepository.DoesUserExistAsync(userId);
        }

        public async Task<IEnumerable<MovieModel>> GetMoviesAsync(FilterModel filters)
        {
            var movies = new List<Movie>(await _movieRepository.GetMoviesAsync(filters));
            var userRatings = new List<UserRating>(await _movieRepository.GetRatingsAsync(movies));
            var movieRatings = CollateRatings(userRatings);
            var movieModels = CombineMovieAndRating(movies, movieRatings);
            return movieModels
                .OrderBy(m => m.Title)
                .ToList();
        }

        public async Task<IEnumerable<MovieModel>> GetTopFiveMoviesAsync()
        {
            var userRatings = new List<UserRating>(await _movieRepository.GetAllMovieRatingsAsync());
            return await GetTopFiveRatedMovieModels(userRatings);
        }

        public async Task<IEnumerable<MovieModel>> GetUserTopFiveMoviesAsync(int userId)
        {
            var userRatings = new List<UserRating>(await _movieRepository.GetAllMovieRatingsForUserAsync(userId));
            return await GetTopFiveRatedMovieModels(userRatings);
        }

        private async Task<List<MovieModel>> GetTopFiveRatedMovieModels(List<UserRating> userRatings)
        {
            var movieRatings = CollateRatings(userRatings);
            var topRatedMovieIds = GetMovieIdsForTopFiveRatings(movieRatings);
            var topRatedMovies = new List<Movie>(await _movieRepository.GetMoviesByIdsAsync(topRatedMovieIds));
            var topRatedMovieModels = CombineMovieAndRating(topRatedMovies, movieRatings);

            return topRatedMovieModels
                .OrderBy(m => m.Title)
                .OrderByDescending(m => m.AverageRating)
                .Take(5)
                .ToList();
        }
        
        public void UpsertUserMovieRating(int movieId, int userId, int rating)
        {
            _movieRepository.UpsertMovieRatingForUser(movieId, userId, rating);
        }

        private List<int> GetMovieIdsForTopFiveRatings(List<MovieRatingModel> movieRatings)
        {
            var topFiveRatings = movieRatings
                .OrderByDescending(o => o.AverageRating)
                .Distinct()
                .Select(o => o.AverageRating)
                .Take(5)
                .ToList();

            return movieRatings
                .Where(m => topFiveRatings.Contains(m.AverageRating))
                .Select(m => m.MovieId)
                .ToList();
        }

        private List<MovieModel> CombineMovieAndRating(List<Movie> movies, List<MovieRatingModel> ratings)
        {
            return movies.Select(m => new MovieModel
            {
                Id = m.Id,
                Title = m.Title,
                RunningTime = m.RunningTime,
                YearOfRelease = m.YearOfRelease,
                AverageRating = ratings.Where(a => a.MovieId == m.Id).Select(a => a.AverageRating).FirstOrDefault()
            }).ToList();
        }

        private List<MovieRatingModel> CollateRatings(List<UserRating> userRatings)
        {
            return userRatings
                .GroupBy(m => m.MovieId)
                .Select(m => new MovieRatingModel
                {
                    MovieId = m.Key,
                    NumberOfRatings = m.Count(),
                    TotalOfRatings = m.Sum(x => x.Rating)
                })
                .ToList();
        }

        #region IDisposable

        private bool _disposed;

        ~MovieService()
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
    }
}
