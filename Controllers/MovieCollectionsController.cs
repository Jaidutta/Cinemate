using Cinemate.Data;
using Cinemate.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Cinemate.Controllers
{
    public class MovieCollectionsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public MovieCollectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /* There is only 1 Index action. There are 2 version of it. HttpGet and HttpPost
         *  because of the way we have built to illustrate how much freedom we have got when building out the
         *  internal requests and user redirects
         */
        public async Task<IActionResult> Index(int? id)
        { id ??= (await _context.Collection.FirstOrDefaultAsync(c => c.Name.ToUpper() == "ALL")).Id;

            //Step 0: Show the currently selected Collection in the dropdown

            /* This will fill in the Collection dropdown and setting the default selected value
             * Using ViewData to select the information for a dropdown list is fairly common in Mvc 
             
                                      1.Information in select list will be all Collection record
                                      2. selected  "Id" will be posted when the form is submitted 
                                      3. Name will be displayed in dropdown
                                      4. Currently selected item will be based of the 4 param, the id
            */
            ViewData["CollectionId"] = new SelectList(_context.Collection, "Id", "Name", id);

            //Step 1: Get a list of all the movie id's in the system
            var allMovieIds = await _context.Movie.Select(m => m.Id).ToListAsync();

            //Step 2: Get a list of all the movie id's in the current Collection in reverse Order
            // We are using reverse order because the way it loads up the MultiSelect
            var movieIdsInCollection = await _context.MovieCollection
                                        .Where(m => m.CollectionId == id)
                                        .OrderBy(m => m.Order)
                                        .Select(m => m.MovieId)
                                        .ToListAsync();

            //Step 3: Build a list of Id's not in the Collection
            var movieIdsNotInCollection = allMovieIds.Except(movieIdsInCollection);

            //Step 4: Build a multiselect using movieIdsInCollection
            //Because of the way we are ordering the movie collection records I will be adding each movie inside a loop
            var moviesInCollection = new List<Movie>();
            movieIdsInCollection.ForEach(movieId => moviesInCollection.Add(_context.Movie.Find(movieId)));

            //var moviesInCollection = await _context.Movie.Where(m => movieIdsInCollection.Contains(m.Id)).ToListAsync();
            ViewData["IdsInCollection"] = new MultiSelectList(moviesInCollection, "Id", "Title");

            //Step 5: Build a multiselect using movieIdsNotInCollection
            var moviesNotInCollection = await _context.Movie.AsNoTracking().Where(m => movieIdsNotInCollection.Contains(m.Id)).ToListAsync();
            ViewData["IdsNotInCollection"] = new MultiSelectList(moviesNotInCollection, "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, List<int> idsInCollection)
        {
            // An update of a MovieCollection means the following;
            //  Step 1: Remove all of the movies in the given Collection (collectionId)
            var oldRecords = _context.MovieCollection.Where(c => c.CollectionId == id);
            _context.MovieCollection.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();

            //  Step 2: Add all of the movies indicated in the incoming list of movie ids
            if (idsInCollection != null)
            {
                int index = 1;
                idsInCollection.ForEach(movieId =>
                {
                    _context.Add(new MovieCollection()
                    {
                        CollectionId = id,
                        MovieId = movieId,
                        Order = index++
                    });
                });

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { id });
        }

    }
}
