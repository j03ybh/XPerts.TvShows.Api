using XPerts.TvShows.Models;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public interface ITvShowSyncWriter
    {
        Task<IEnumerable<Genre>> GetOrCreateGenresIfNotExistAsync(IEnumerable<string> genreNames);

        Task AddTvShowsAsync(long latestTvShowIdInDatabase, TvShow[] shows);
    }
}