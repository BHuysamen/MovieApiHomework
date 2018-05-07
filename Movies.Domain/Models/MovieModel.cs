namespace Movies.Domain.Models
{
    public class MovieModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public double AverageRating { get; set; }
    }
}
