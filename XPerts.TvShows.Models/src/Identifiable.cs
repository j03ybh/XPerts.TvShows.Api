namespace XPerts.TvShows.Models
{
    public abstract class Identifiable : IIdentifiable
    {
        public long Id { get; set; }
    }
}