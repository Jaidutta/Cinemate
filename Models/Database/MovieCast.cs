namespace Cinemate.Models.Database
{
    // It will be storing a portion of the credit data when we make a request for movie detail data
    // The credit deta contains both cast and crew information but this model will only be
    // concerning itself with the cast info
    public class MovieCast
    {
        public int Id { get; set; } // primary key 
        public int MovieId { get; set; } // foreign key


        // CastID assigned by TMDB api, will be used when we want to obtain detailed info for a cast or crew member
        // this is the id we will need to provide
        public int CastID { get; set; }  
        public string? Department { get; set; } // department that this particular cast member works in
        public string? Name { get; set; } // name of the cast member
        public string? Character { get; set; } // character played by the cast member

        /* This is the property that stores the full path to an image online for this particular cast member */
        public string? ImageUrl { get; set; } 

        public Movie? Movie { get; set; } // navigation property
    }

}
