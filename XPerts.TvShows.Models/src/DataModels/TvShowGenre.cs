namespace XPerts.TvShows.Models
{
    public class TvShowGenre
    {
        public long TvShowId { get; set; }

        public long GenreId { get; set; }

        public TvShow TvShow { get; set; }

        public Genre Genre { get; set; }
    }
}