using Cinemate.Enums;
using Cinemate.Models.TMDB;

namespace Cinemate.Services.Interfaces
{
    public interface IRemoteMovieService
    {
        Task<MovieDetail> MovieDetailAsync(int id);

                                           //moviecategory --> enum, count -> no of movies in this category we are interested in
        Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count);

        Task<ActorDetail> ActorDetailAsync(int id);
    }

    /* The reason we are using the inteface is because currently we are using 
     * TMDB API but if in future if we want to use any other API, we can implement
     * other sets of classes, geared towards that API 
     */
}
