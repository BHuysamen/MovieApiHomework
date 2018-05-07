using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Domain.Contracts
{
    public interface IMovieService : IDisposable
    {

        bool ValidateMovieFilters(FilterModel filters);
        bool ValidateMovieRating(int rating);
        Task<bool> ValidateMovieIdAsync(int movieId);
        Task<bool> ValidateUserIdAsync(int userId);

        Task<IEnumerable<MovieModel>> GetMoviesAsync(FilterModel filters);
        Task<IEnumerable<MovieModel>> GetTopFiveMoviesAsync();
        Task<IEnumerable<MovieModel>> GetUserTopFiveMoviesAsync(int userId);

        void UpsertUserMovieRating(int movieId, int userId, int rating);
    }
}
