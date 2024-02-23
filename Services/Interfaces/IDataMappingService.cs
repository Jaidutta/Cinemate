using Cinemate.Models.Database;
using Cinemate.Models.TMDB;

namespace Cinemate.Services.Interfaces
{
    /* This is used for mapping and transforming an arbitrary subset of data 
     * coming from the API, in order to use in order to put it to use in my 
     * custom application
     */
    public interface IDataMappingService
    {
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);
    }
}
