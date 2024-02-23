namespace Cinemate.Models.Database
{
    // We are using this to categorise our collection of movies into custom collections
    public class Collection
    {
        public int Id { get; set; } // primary key of the collection table
        public string? Name { get; set; } // name of the collection
        public string? Description { get; set; }


        // navigational property
        public ICollection<MovieCollection> MovieCollection { get; set; } = new HashSet<MovieCollection>();

    }
}
