namespace XPerts.TvShows.Models
{
    public class TvShow : Indexable
    {
        public TvShow()
        {
        }

        public string Name { get; set; }

        public DateTime PremieredOn { get; set; }

        public IEnumerable<TvShowGenre> Genres { get; set; }

        public long? OriginId { get; set; }
    }
}