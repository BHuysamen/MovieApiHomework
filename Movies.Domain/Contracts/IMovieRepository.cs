using Movies.Domain.Entities;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Domain.Contracts
{
    public interface IMovieRepository : IDisposable
    {
        Task<bool> DoesMovieExistAsync(int movieId);
        Task<bool> DoesUserExistAsync(int userId);
        Task<IEnumerable<Movie>> GetMoviesAsync(FilterModel filters);
        Task<IEnumerable<Movie>> GetMoviesByIdsAsync(List<int> movies);
        Task<IEnumerable<UserRating>> GetRatingsAsync(List<Movie> movies);
        Task<IEnumerable<UserRating>> GetAllMovieRatingsAsync();
        Task<IEnumerable<UserRating>> GetAllMovieRatingsForUserAsync(int userId);
        void UpsertMovieRatingForUser(int movieId, int userId, int rating);
    }
}
