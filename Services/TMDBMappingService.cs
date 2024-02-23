using Cinemate.Enums;
using Cinemate.Models.Database;
using Cinemate.Models.Settings;
using Cinemate.Models.TMDB;
using Cinemate.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Cinemate.Services
{
    public class TMDBMappingService : IDataMappingService
    {
        private readonly AppSettings _appSettings;
        private readonly IImageService _imageService;

        public TMDBMappingService(IOptions<AppSettings> appSettings, IImageService imageService)
        {
            _appSettings = appSettings.Value;
            _imageService = imageService;
        }



        public ActorDetail MapActorDetail(ActorDetail actor)
        {
            // 1.Image
            actor.profile_path = BuildCastImage(actor.profile_path);

            // 2. Bio
            if (string.IsNullOrEmpty(actor.biography))
            {
                actor.biography = "Not Available";
            }
            else
            {
                actor.biography = actor.biography.Trim();
            }

            // 3. Place of Birth
            if (string.IsNullOrEmpty(actor.place_of_birth))
                actor.place_of_birth = "Not Available";

            // 4. Birthday
            if (string.IsNullOrEmpty(actor.birthday))
                actor.birthday = "Not Available";
            else
                actor.birthday = DateTime.Parse(actor.birthday).ToString("MMM dd, yyyy");

            return actor;
        }

        public async Task<Movie> MapMovieDetailAsync(MovieDetail movie)
        {
            Movie newMovie = null;

            try
            {
                newMovie = new Movie()
                {
                    MovieId = movie.id,
                    Title = movie.title,
                    TagLine = movie.tagline,
                    Overview = movie.overview,
                    RunTime = movie.runtime,
                    VoteAverage = movie.vote_average,
                    ReleaseDate = DateTime.Parse(movie.release_date),
                    TrailerUrl = BuildTrailerPath(movie.videos),
                    Backdrop = await EncodeBackdropImageAsync(movie.backdrop_path),
                    BackdropType = BuildImageType(movie.backdrop_path),
                    Poster = await EncodePosterImageAsync(movie.poster_path),
                    PosterType = BuildImageType(movie.poster_path),
                    Rating = GetRating(movie.release_dates)

                };

                //----- Cast Members ------
                var castMembers = movie.credits.cast.OrderByDescending(c => c.popularity)
                                                    .GroupBy(c => c.cast_id)
                                                    .Select(g => g.FirstOrDefault())
                                                    .Take(20)
                                                    .ToList();

                castMembers.ForEach(member =>
                {
                    newMovie.Cast.Add(new MovieCast()
                    {
                        CastID = member.id,
                        Department = member.known_for_department,
                        Name = member.name,
                        Character = member.character,
                        ImageUrl = BuildCastImage(member.profile_path)
                    });
                });


                //----- Crew Members ------
                var crewMembers = movie.credits.crew.OrderByDescending(c => c.popularity)
                                                    .GroupBy(c => c.id)
                                                    .Select(g => g.First())
                                                    .Take(20)
                                                    .ToList();

                crewMembers.ForEach(member =>
                {
                    newMovie.Cast.Add(new MovieCast()
                    {
                        CastID = member.id,
                        Department = member.department,
                        Name = member.name,
                        Character = member.job,
                        ImageUrl = BuildCastImage(member.profile_path)
                    });
                });

            }
            catch (Exception ex)
            {
                Console.Write($"Excetion in MapMovieDetailAsync:  {ex.Message}");
            }

            return newMovie;

        }

        public string BuildCastImage(string profilePath)
        {
            if (string.IsNullOrEmpty(profilePath))
                return _appSettings.CinemateSettings.DefaultCastImage;

            return $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.CinemateSettings.DefaultPosterSize}/{profilePath}";
        }


        private string BuildImageType(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            return $"image/{Path.GetExtension(path).TrimStart('.')}";
        }


        private MovieRating GetRating(Release_Dates dates)
        {
            var movieRating = MovieRating.NR;
            var certification = dates.results.FirstOrDefault(r => r.iso_3166_1 == "US");
            if (certification != null)
            {
                var apiRating = certification.release_dates.FirstOrDefault(c => c.certification != "")?.certification.Replace("-", "");
                if (!string.IsNullOrEmpty(apiRating))
                {
                    movieRating = (MovieRating)Enum.Parse(typeof(MovieRating), apiRating, true);
                }
            }
            return movieRating;
        }

        private async Task<byte[]> EncodePosterImageAsync(string path)
        { 
            // It is very similar to EncodeBackdropImageAsync but it uses a different appSetting values
            var posterPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.CinemateSettings.DefaultPosterSize}/{path}";
            return await _imageService.EncodeImageURLAsync(posterPath);
        }
        private string BuildTrailerPath(Videos videos) { 

            // // 1. Find the first video result that is a trailer and has a non-empty key.
            var videoKey = videos.results.FirstOrDefault(r => r.type.ToLower().Trim() == "trailer" && r.key != "")?.key;
            return string.IsNullOrEmpty(videoKey) ? videoKey : $"{_appSettings.TMDBSettings.BaseYoutubePath}{videoKey}";
        }

        private async Task<byte[]> EncodeBackdropImageAsync(string path)
        {
            var backdropPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.CinemateSettings.DefaultBackdropSize}/{path}";
            return await _imageService.EncodeImageURLAsync(backdropPath);
        }
    }
}
