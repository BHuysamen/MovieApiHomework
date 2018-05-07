using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Domain.Entities
{
    public class UserRating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Rating { get; set; }
    }
}
