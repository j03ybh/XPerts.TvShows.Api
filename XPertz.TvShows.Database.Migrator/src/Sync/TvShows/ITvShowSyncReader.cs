namespace XPertz.TvShows.Database.Migrator.Sync
{
    public interface ITvShowSyncReader
    {
        Task<long> GetLatestAddedSyncedTvShowIdInDatabaseAsync();
    }
}