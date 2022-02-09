using Microsoft.Extensions.Logging;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Database;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public class TvShowSyncReader : ITvShowSyncReader
    {
        private readonly ILogger _logger;
        private readonly IStatementExecutor _statementExecutor;

        public TvShowSyncReader(ILogger<TvShowSyncReader> logger, IStatementExecutor statementExecutor)
        {
            _logger = logger;
            _statementExecutor = statementExecutor;
        }

        public async Task<long> GetLatestAddedSyncedTvShowIdInDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Getting the id of the latest show that was added during the last sync.");

                var hasSyncedEntries = _statementExecutor.ExecuteScalar<bool>($@"
                    IF(EXISTS(SELECT 1 FROM {TvShowTableConfiguration.Name} WHERE ManuallyCreated = 0 AND OriginId IS NOT NULL))
                        BEGIN
                           SELECT CAST(1 AS BIT)
                        END
                    ELSE
                        BEGIN
                           SELECT CAST(0 AS BIT)
                        END
                ");
                if (!hasSyncedEntries)
                {
                    _logger.LogInformation("No previous syncs have completed, the id is set to 0");
                    return 0;
                }

                var id = _statementExecutor.ExecuteScalar<long>($@"
                    SELECT OriginId
                    FROM {TvShowTableConfiguration.Name}
                    WHERE OriginId = (
                        SELECT max(OriginId) FROM {TvShowTableConfiguration.Name}
                    )
                    AND ManuallyCreated = 0 AND OriginId IS NOT NULL
                ");

                _logger.LogInformation("The id of the latest synced record is {id}", id);

                return await Task.FromResult(id).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to read latest id");
                return -1;
            }
        }
    }
}