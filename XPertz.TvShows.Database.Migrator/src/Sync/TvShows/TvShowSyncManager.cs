using Microsoft.Extensions.Logging;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public class TvShowSyncManager : ITvShowSyncManager
    {
        private readonly ILogger _logger;
        private readonly IMazeTvApiClient _mazeApiClient;
        private readonly ITvShowSyncReader _reader;
        private readonly ITvShowSyncFilter _filter;
        private readonly ITvShowSyncWriter _writer;

        public TvShowSyncManager(ILogger<TvShowSyncManager> logger, IMazeTvApiClient mazeApiClient, ITvShowSyncReader reader, ITvShowSyncFilter filter, ITvShowSyncWriter writer)
        {
            _logger = logger;
            _mazeApiClient = mazeApiClient;
            _reader = reader;
            _filter = filter;
            _writer = writer;
        }

        public async Task SyncAsync()
        {
            var lastAddedSyncedTvShowIdInDatabase = await _reader
                    .GetLatestAddedSyncedTvShowIdInDatabaseAsync()
                    .ConfigureAwait(false);

            if (lastAddedSyncedTvShowIdInDatabase > -1)
            {
                var mazeApiStartPageNumber = (int)Math.Round((double)lastAddedSyncedTvShowIdInDatabase / 250);

                var tvShows = await _mazeApiClient
                    .GetTvShowsAsync(mazeApiStartPageNumber)
                    .ConfigureAwait(false);

                if (tvShows.Length > 0)
                {
                    var filteredTvShows = _filter.Filter(tvShows);

                    _logger.LogInformation("Adding the genres if they do not exist yet.");
                    var allGenres = filteredTvShows.SelectMany(x => x.Genres);
                    var genres = await _writer
                        .GetOrCreateGenresIfNotExistAsync(allGenres)
                        .ConfigureAwait(false);

                    _logger.LogInformation("Adding the tv shows and their genres.");
                    var tvShowsToAdd = PrepareTvShowsToAdd(filteredTvShows, genres);
                    await _writer
                        .AddTvShowsAsync(lastAddedSyncedTvShowIdInDatabase, tvShowsToAdd)
                        .ConfigureAwait(false);
                }
            }
        }

        private static TvShow[] PrepareTvShowsToAdd(TvShowView[] views, IEnumerable<Genre> genres)
        {
            return views.Select(view =>
            {
                var tvShowGenres = genres
                    .Where(x => view.Genres.Any(y => y == x.Name))
                    .Select(x => new TvShowGenre
                    {
                        Genre = x,
                        GenreId = x.Id
                    });

                return new TvShow
                {
                    OriginId = view.Id,
                    Genres = tvShowGenres,
                    Name = view.Name,
                    PremieredOn = DateTime.TryParse(view.PremieredOn, out var premieredOn)
                        ? premieredOn
                        : throw new InvalidOperationException($"The premiered on date of the tv show with name '{view.Name}' could not be parsed to a valid date time value.")
                };
            })
            .OrderBy(x => x.Name)
            .ToArray();
        }
    }
}