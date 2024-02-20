using Cinemate.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemate.Models.Database
{
    public class Movie
    {
        public int Id { get; set; }

        public int MovieId { get; set; }  // id of the movie, as it sits in TMBDB's db

        public string? Title { get; set; } // for movie title

        public string? Tagline { get; set; } // tagline of the movie as it come from the Api

        public string? Overview { get; set; }

        public string? Runtime { get; set; } // used to store runtime of the movie


        [DataType(DataType.Date)]
        /* Telling the UI when it prompts user, 
         * even though the underlying type is DateTime don't bother requesting the time portion
         * what happens time will be stored but it will be stored as 0
         */

        [Display(Name="Release Date")]
        public DateTime ReleaseDate { get; set; }


        public MovieRating Rating { get; set; }

        public float VoteAverage { get; set; }

        public byte[]? Poster { get; set; } // Smaller image on movieDetail page
        public string? PosterType { get; set; }

        public byte[]? Backdrop { get; set; } // Full width image shown at top of movie detail page
        public string? BackdropType { get; set; }

        public string? TrailerUrl { get; set; } // this will take us to some trailer on Youtube

        [NotMapped]
        [Display(Name ="Poster Image")]
        public IFormFile? PosterFile { get; set; } // user selection to store a Poster file

        [NotMapped]
        [Display(Name = "Poster Image")]
        public IFormFile? BackdropFile { get; set; } // user selection to store a Backdrop file

        public ICollection<MovieCollection> Collections { get; set; } = new HashSet<MovieCollection>();
        public ICollection<MovieCast> Cast { get; set; } = new HashSet<MovieCast>();
        public ICollection<MovieCrew> Crew { get; set; } = new HashSet<MovieCrew>();
    }
}
