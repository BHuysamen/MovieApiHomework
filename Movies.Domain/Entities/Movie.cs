using System.Collections.Generic;

namespace Movies.Domain.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public ICollection<MovieGenre> MovieGenres { get; } = new List<MovieGenre>();
    }
}
