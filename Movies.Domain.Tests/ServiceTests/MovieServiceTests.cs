using FakeItEasy;
using Movies.Domain.Contracts;
using Movies.Domain.Entities;
using Movies.Domain.Models;
using Movies.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Movies.Domain.Tests.ServiceTests
{
    public class MovieServiceTests
    {
        [Fact]
        public void ValidateMovieFilters_NullFilters_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(null);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_EmptyFilters_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel();

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_NegativeYear_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel()
            {
                Year = -2018,
                Title = "The",
                Genres = new List<string> { "Horror", "Sci-Fi" }
            };

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_EmptyGenre_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel() {
                Year = 2018,
                Title = "The",
                Genres = new List<string> { "Horror", "" } };

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_NullGenre_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel()
            {
                Year = 2018,
                Title = "The",
                Genres = new List<string> { "Horror", null }
            };

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_ValidTitle_ReturnsTrue()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel()
            {
                Year = 0,
                Title = "The",
                Genres = null
            };

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.True(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_ValidYear_ReturnsTrue()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel()
            {
                Year = 1998,
                Title = null,
                Genres = new List<string>()
            };

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.True(result);
            }
        }

        [Fact]
        public void ValidateMovieFilters_ValidGenres_ReturnsTrue()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel()
            {
                Year = 0,
                Title = null,
                Genres = new List<string> { "Horror" }
            };

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieFilters(filter);

                // Then
                Assert.True(result);
            }
        }

        [Fact]
        public async void GetMoviesAsync_ValidFilters_ReturnsMovies()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();
            var filter = new FilterModel()
            {
                Year = 0,
                Title = null,
                Genres = new List<string> { "Horror" }
            };
            A.CallTo(() => movieRepo.GetMoviesAsync(filter)).Returns(MakeFakeMovieList());
            A.CallTo(() => movieRepo.GetRatingsAsync(A<List<Movie>>.Ignored)).Returns(MakeFakeUserRatingList());

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var movies = await movieService.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<MovieModel>(movies);
                var movie = movieList.Where(m => m.Id == 2).FirstOrDefault();
                Assert.Equal(4.5, movie.AverageRating);
            }
        }

        [Fact]
        public void ValidateMovieRating_RatingTooLow_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieRating(-1);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieRating_RatingTooHigh_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieRating(6);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public void ValidateMovieRating_ValidRating_ReturnsFalse()
        {
            // Given
            var movieRepo = A.Fake<IMovieRepository>();

            // When
            using (var movieService = new MovieService(movieRepo))
            {
                var result = movieService.ValidateMovieRating(5);

                // Then
                Assert.True(result);
            }
        }

        private List<Movie> MakeFakeMovieList()
        {
            return new List<Movie>
            {
                new Movie { Id = 1, Title = "A Movie", YearOfRelease = 1998, RunningTime = 120 },
                new Movie { Id = 2, Title = "AB Movie", YearOfRelease = 2001, RunningTime = 120 }
            };
        }

        private List<UserRating> MakeFakeUserRatingList()
        {
            return new List<UserRating>
            {
                new UserRating { MovieId = 1, UserId = 1, Rating = 2 },
                new UserRating { MovieId = 2, UserId = 1, Rating = 3 },
                new UserRating { MovieId = 1, UserId = 2, Rating = 2 },
                new UserRating { MovieId = 2, UserId = 2, Rating = 5 },
                new UserRating { MovieId = 1, UserId = 3, Rating = 2 },
                new UserRating { MovieId = 2, UserId = 3, Rating = 5 },
                new UserRating { MovieId = 1, UserId = 4, Rating = 2 },
                new UserRating { MovieId = 2, UserId = 4, Rating = 5 }
            };
        }
    }
}
