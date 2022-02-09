using XPerts.TvShows.Models;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public interface ITvShowSyncFilter
    {
        TvShowView[] Filter(IEnumerable<TvShowView> shows);
    }
}