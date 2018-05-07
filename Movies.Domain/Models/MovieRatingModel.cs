using System;

namespace Movies.Domain.Models
{
    public class MovieRatingModel
    {
        public int MovieId { get; set; }
        public double NumberOfRatings { get; set; }
        public double TotalOfRatings { get; set; }
        public double AverageRating { get { return Math.Round((TotalOfRatings / NumberOfRatings) * 2) / 2; } }
    }
}
