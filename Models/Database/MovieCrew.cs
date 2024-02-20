namespace Cinemate.Models.Database
{
    // The other half of the cast and crew data that we get when we request for the movie detail data
    public class MovieCrew
    {


        public int Id { get; set; }  // primary key
        public int MovieId { get; set; }  // foreign key

        /*CrewID assigned by TMDB api, will be used when we want to obtain detailed info for a crew member
         this is the id we will need to provide */
        public int CrewID { get; set; }  

        public string? Department { get; set; } // department of the crew member
        public string? Name { get; set; }  // name of the crew member
        public string? Job { get; set; }  // job of the crew member in the context of this movie
        public string? ImageUrl { get; set; } // path to an online image representing this crew member

        public Movie? Movie { get; set; } // navigational property
    }
}
