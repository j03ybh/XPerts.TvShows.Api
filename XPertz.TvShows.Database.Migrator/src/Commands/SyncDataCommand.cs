using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using XPertz.TvShows.Database.Migrator.Sync;

namespace XPertz.TvShows.Database.Migrator
{
    [Command(Name = "sync-data")]
    internal sealed class SyncDataCommand : CommandBase
    {
        private readonly ILogger _logger;
        private readonly ITvShowSyncManager _syncManager;

        public SyncDataCommand(IConsole console, ILogger<SyncDataCommand> logger, ITvShowSyncManager syncManager)
            : base(console)
        {
            _logger = logger;
            _syncManager = syncManager;
        }

        protected override async Task<int> OnInnerExecuteAsync(CommandLineApplication app)
        {
            try
            {
                await _syncManager
                    .SyncAsync()
                    .ConfigureAwait(false);

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync data");
            }

            return 1;
        }
    }
}