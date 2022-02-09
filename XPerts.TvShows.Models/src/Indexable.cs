namespace XPerts.TvShows.Models
{
    public abstract class Indexable : Identifiable, IIndexable
    {
        public long IndexPosition { get; set; }
    }
}