using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;

namespace Movies.Domain.Repositories
{
    public class MovieDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<User> Users { get; set; }

        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasKey(g => new { g.Id });
            modelBuilder.Entity<Movie>().HasKey(m => new { m.Id });
            modelBuilder.Entity<User>().HasKey(u => new { u.Id });
            modelBuilder.Entity<MovieGenre>().HasKey(mg => new { mg.MovieId, mg.GenreId });
            modelBuilder.Entity<UserRating>().HasKey(ur => new { ur.UserId, ur.MovieId });
        }
    }
}
