using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Domain.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<MovieGenre> MovieGenres { get; } = new List<MovieGenre>();
    }
}
