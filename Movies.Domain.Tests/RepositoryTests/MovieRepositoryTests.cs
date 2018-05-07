using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Movies.Domain.Repositories;
using Movies.Domain.Entities;
using Movies.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Movies.Domain.Tests.RepositoryTests
{
    public class MovieRepositoryTests
    {
        private readonly DbContextOptions<MovieDbContext> _options;

        public MovieRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<MovieDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async void GetMoviesAsync_WithYearFilter_Returns4Movies()
        {
            // Given
            var filter = new FilterModel() { Year = 2001, Title = null, Genres = null };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }
            
            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var movies = await movieRepo.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<Movie>(movies);
                Assert.Equal(4, movieList.Count);
            }
        }

        [Fact]
        public async void GetMoviesAsync_WithTitleFilter_Returns6Movies()
        {
            // Given
            var filter = new FilterModel() { Year = 0, Title = "A", Genres = null };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var movies = await movieRepo.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<Movie>(movies);
                Assert.Equal(6, movieList.Count);
            }
        }

        [Fact]
        public async void GetMoviesAsync_WithTitleAndYearFilter_Returns2Movies()
        {
            // Given
            var filter = new FilterModel() { Year = 2001, Title = "A", Genres = null };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var movies = await movieRepo.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<Movie>(movies);
                Assert.Equal(2, movieList.Count);
            }
        }

        [Fact]
        public async void GetMoviesAsync_GenreFilter_Returns8Movies()
        {
            // Given
            var filter = new FilterModel() { Year = 0, Title = null, Genres = new List<string> { "Horror" } };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var movies = await movieRepo.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<Movie>(movies);
                Assert.Equal(8, movieList.Count);
            }
        }

        [Fact]
        public async void GetMoviesAsync_GenreAndYearFilter_Returns4Movies()
        {
            // Given
            var filter = new FilterModel() { Year = 2001, Title = null, Genres = new List<string> { "Horror" } };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var movies = await movieRepo.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<Movie>(movies);
                Assert.Equal(4, movieList.Count);
            }
        }

        [Fact]
        public async void GetMoviesAsync_GenreAndYearAndTitleFilter_Returns2Movies()
        {
            // Given
            var filter = new FilterModel() { Year = 2001, Title = "A", Genres = new List<string> { "Horror" } };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var movies = await movieRepo.GetMoviesAsync(filter);

                // Then
                Assert.NotNull(movies);
                Assert.NotEmpty(movies);
                var movieList = new List<Movie>(movies);
                Assert.Equal(2, movieList.Count);
            }
        }

        [Fact]
        public async void GetRatingsAsync_ForAnEmptyListOfMovies_ReturnsAnEmptyList()
        {
            // Given
            var movies = new List<Movie>();
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var ratings = await movieRepo.GetRatingsAsync(movies);

                // Then
                Assert.NotNull(ratings);
                Assert.Empty(ratings);
            }
        }

        [Fact]
        public async void GetRatingsAsync_ForAListOfMovies_ReturnsRatings()
        {
            // Given
            var movies = new List<Movie> { new Movie { Id = 1, Title = "A Movie", YearOfRelease = 1998, RunningTime = 120 },
                                           new Movie { Id = 2, Title = "AB Movie", YearOfRelease = 2001, RunningTime = 120 }};
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var ratings = await movieRepo.GetRatingsAsync(movies);

                // Then
                Assert.NotNull(ratings);
                Assert.NotEmpty(ratings);
                var ratingList = new List<UserRating>(ratings);
                Assert.Equal(6, ratingList.Count);
            }
        }

        [Fact]
        public async void DoesMovieExistAsync_InvalidId_ReturnsFalse()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.DoesMovieExistAsync(200);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public async void DoesMovieExistAsync_ValidId_ReturnsTrue()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.DoesMovieExistAsync(2);

                // Then
                Assert.True(result);
            }
        }

        [Fact]
        public async void DoesUserExistAsync_InvalidId_ReturnsFalse()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.DoesUserExistAsync(200);

                // Then
                Assert.False(result);
            }
        }

        [Fact]
        public async void DoesUserExistAsync_ValidId_ReturnsTrue()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.DoesUserExistAsync(2);

                // Then
                Assert.True(result);
            }
        }

        [Fact]
        public async void GetMoviesByIdsAsync_EmptyList_ReturnsEmptyList()
        {
            // Given
            var movieIds = new List<int>();
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.GetMoviesByIdsAsync(movieIds);

                // Then
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async void GetMoviesByIdsAsync_PopulatedList_ReturnsAllResults()
        {
            // Given
            var movieIds = new List<int> { 1, 2, 3, 4, 5 };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.GetMoviesByIdsAsync(movieIds);

                // Then
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                var movieList = new List<Movie>(result);
                Assert.Equal(5, movieList.Count);
            }
        }

        [Fact]
        public async void GetMoviesByIdsAsync_WithInvalidIdInList_ReturnsAllValidResults()
        {
            // Given
            var movieIds = new List<int> { 1, 2, 3, 4, 200 };
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.GetMoviesByIdsAsync(movieIds);

                // Then
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                var movieList = new List<Movie>(result);
                Assert.Equal(4, movieList.Count);
            }
        }

        [Fact]
        public async void GetAllMovieRatingsAsync_ReturnsAllRatings()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.GetAllMovieRatingsAsync();

                // Then
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                var ratingsList = new List<UserRating>(result);
                Assert.Equal(38, ratingsList.Count);
            }
        }

        [Fact]
        public async void GetAllMovieRatingsForUserAsync_InvalidUser_ReturnsEmptyList()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.GetAllMovieRatingsForUserAsync(200);

                // Then
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async void GetAllMovieRatingsForUserAsync_ValidUser_ReturnsAllRatings()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                BuildMoviesContext(context);
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                var result = await movieRepo.GetAllMovieRatingsForUserAsync(1);

                // Then
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                var ratingsList = new List<UserRating>(result);
                Assert.Equal(9, ratingsList.Count);
            }
        }

        [Fact]
        public async void UpsertMovieRatingForUser_ValidExistingArguments_UpdatesRating()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                context.Movies.Add(new Movie { Id = 20, Title = "A Movie", YearOfRelease = 1998, RunningTime = 120 });
                context.Users.Add(new User { Id = 5 });
                context.UserRatings.Add(new UserRating { MovieId = 20, UserId = 5, Rating = 2 });
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                movieRepo.UpsertMovieRatingForUser(20, 5, 1);

                // Then
                var rating = context.UserRatings.Where(u => u.UserId == 5 && u.MovieId == 20).FirstOrDefault();
                Assert.NotNull(rating);
                Assert.Equal(1, rating.Rating);
            }
        }

        [Fact]
        public async void UpsertMovieRatingForUser_ValidArguments_UpdatesRating()
        {
            // Given
            using (var context = new MovieDbContext(_options))
            {
                context.Movies.Add(new Movie { Id = 21, Title = "A Movie", YearOfRelease = 1998, RunningTime = 120 });
                context.Users.Add(new User { Id = 5 });
                await context.SaveChangesAsync();
            }

            using (var context = new MovieDbContext(_options))
            using (var movieRepo = new MovieRepository(context))
            {
                // When
                movieRepo.UpsertMovieRatingForUser(21, 5, 4);

                // Then
                var rating = context.UserRatings.Where(u => u.UserId == 5 && u.MovieId == 21).FirstOrDefault();
                Assert.NotNull(rating);
                Assert.Equal(4, rating.Rating);
            }
        }



        private void BuildMoviesContext(MovieDbContext context)
        {
            context.Movies.Add(new Movie { Id = 91, Title = "A Movie", YearOfRelease = 1998, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 92, Title = "AB Movie", YearOfRelease = 2001, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 93, Title = "C Movie", YearOfRelease = 2001, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 94, Title = "AD Movie", YearOfRelease = 2002, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 95, Title = "E Movie", YearOfRelease = 2007, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 96, Title = "F Movie", YearOfRelease = 2011, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 97, Title = "G Movie", YearOfRelease = 2014, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 98, Title = "H Movie", YearOfRelease = 2015, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 99, Title = "I Movie", YearOfRelease = 2017, RunningTime = 120 });
            context.Movies.Add(new Movie { Id = 100, Title = "J Movie", YearOfRelease = 2018, RunningTime = 120 });

            context.Genres.Add(new Genre { Id = 91, Name = "Comedy" });
            context.Genres.Add(new Genre { Id = 92, Name = "Action" });
            context.Genres.Add(new Genre { Id = 93, Name = "Thriller" });
            context.Genres.Add(new Genre { Id = 94, Name = "Horror" });
            context.Genres.Add(new Genre { Id = 95, Name = "Sci-Fi" });

            context.MovieGenres.Add(new MovieGenre { MovieId = 91, GenreId = 91 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 92, GenreId = 94 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 93, GenreId = 94 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 94, GenreId = 94 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 95, GenreId = 95 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 96, GenreId = 91 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 97, GenreId = 92 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 98, GenreId = 93 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 99, GenreId = 94 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 100, GenreId = 95 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 91, GenreId = 92 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 92, GenreId = 93 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 93, GenreId = 95 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 94, GenreId = 93 });
            context.MovieGenres.Add(new MovieGenre { MovieId = 95, GenreId = 91 });

            context.UserRatings.Add(new UserRating { MovieId = 91, UserId = 91, Rating = 92 });
            context.UserRatings.Add(new UserRating { MovieId = 92, UserId = 91, Rating = 93 });
            context.UserRatings.Add(new UserRating { MovieId = 93, UserId = 91, Rating = 94 });
            context.UserRatings.Add(new UserRating { MovieId = 91, UserId = 92, Rating = 92 });
            context.UserRatings.Add(new UserRating { MovieId = 92, UserId = 92, Rating = 95 });
            context.UserRatings.Add(new UserRating { MovieId = 93, UserId = 92, Rating = 93 });
            context.UserRatings.Add(new UserRating { MovieId = 91, UserId = 93, Rating = 92 });
            context.UserRatings.Add(new UserRating { MovieId = 92, UserId = 93, Rating = 95 });
            context.UserRatings.Add(new UserRating { MovieId = 93, UserId = 93, Rating = 91 });
            context.UserRatings.Add(new UserRating { MovieId = 91, UserId = 94, Rating = 92 });
            context.UserRatings.Add(new UserRating { MovieId = 92, UserId = 94, Rating = 95 });

            context.Users.Add(new User { Id = 91 });
            context.Users.Add(new User { Id = 92 });
            context.Users.Add(new User { Id = 93 });
            context.Users.Add(new User { Id = 94 });
        }
    }
}

