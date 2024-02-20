namespace Cinemate.Models.Database
{
    // Many-to-Many Table for Movie and Collection
    // It's job is to hold a bunch of primary keys.
    // Each record in this table will have a movie primary key and collection primary key 
    // Linking the 2 together
    public class MovieCollection
    {
        public int Id { get; set; } // primary key of this table
        public int CollectionId { get; set; } // Foreign key
        public int MovieId { get; set; }      // Foreign Key

        public int Order { get; set; } 
        /*
         * used to order/prioritize movies in collection because a movie can be in more than 
         * 1 collection and it may appear at the top in collection and at the bottom in another collection
         * we will store that info in this property
         */

        public Collection? Collection { get; set; }
        public Movie? Movie { get; set; }

    }
}
