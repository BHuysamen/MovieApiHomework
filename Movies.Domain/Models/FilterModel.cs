using System.Collections.Generic;

namespace Movies.Domain.Models
{
    public class FilterModel
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public List<string> Genres { get; set; }
    }
}
