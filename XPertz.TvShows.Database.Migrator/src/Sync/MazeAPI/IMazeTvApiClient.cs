using XPerts.TvShows.Models;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public interface IMazeTvApiClient
    {
        Task<TvShowView[]> GetTvShowsAsync(int startPage);
    }
}