using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Movies.Domain.Contracts;
using Movies.Domain.Models;

namespace Movies.Service.Controllers
{
    [Route("movies")]
    [ProducesResponseType(500)]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost("ByFilters")]
        [ProducesResponseType(typeof(List<MovieModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMovies([FromBody] FilterModel filters)
        {
            if (!_movieService.ValidateMovieFilters(filters))
                return BadRequest();

            var movies = await _movieService.GetMoviesAsync(filters);

            return ValidateAndReturnMovieResults(movies);
        }

        [HttpGet("User/All/TopFiveRating")]
        [ProducesResponseType(typeof(List<MovieModel>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTopFiveMovies()
        {
            var movies = await _movieService.GetTopFiveMoviesAsync();

            return ValidateAndReturnMovieResults(movies);
        }

        [HttpGet("User/{userId}/TopFiveRating")]
        [ProducesResponseType(typeof(List<MovieModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserTopFiveMovies(int userId)
        {
            if (!await _movieService.ValidateUserIdAsync(userId))
                return BadRequest();

            var movies = await _movieService.GetUserTopFiveMoviesAsync(userId);

            return ValidateAndReturnMovieResults(movies);
        }

        [HttpPut("{movieId}/User/{userId}/Rating/{rating}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpsertUserMovieRating(int movieId, int userId, int rating)
        {
            if (!_movieService.ValidateMovieRating(rating))
                return BadRequest();

            if (!await _movieService.ValidateMovieIdAsync(movieId) || !await _movieService.ValidateUserIdAsync(userId))
                return NotFound();

            _movieService.UpsertUserMovieRating(movieId, userId, rating);                

            return Ok();
        }

        private IActionResult ValidateAndReturnMovieResults(IEnumerable<MovieModel> movies)
        {
            if (movies == null || movies.Count() == 0)
                return NotFound();

            return Ok((List<MovieModel>)movies);
        }
    }
}