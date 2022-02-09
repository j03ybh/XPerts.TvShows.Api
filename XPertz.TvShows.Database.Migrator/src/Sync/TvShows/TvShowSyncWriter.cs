using Microsoft.Extensions.Logging;
using TechMinimalists.Database.Core.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;
using XPertz.TvShows.Database.Migrator.Extensions;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public class TvShowSyncWriter : ITvShowSyncWriter
    {
        private readonly ILogger _logger;
        private readonly IStatementExecutor _statementExecutor;
        private readonly IModelMapper<Genre> _genreDataMapper;
        private readonly IModelMapper<TvShow> _tvShowDataMapper;
        private readonly IModelMapper<TvShowGenre> _tvShowGenreDataMapper;

        public TvShowSyncWriter(ILogger<TvShowSyncWriter> logger, IStatementExecutor statementExecutor, IModelMapper<Genre> genreDataMapper, IModelMapper<TvShow> tvShowDataMapper, IModelMapper<TvShowGenre> tvShowGenreDataMapper)
        {
            _logger = logger;
            _statementExecutor = statementExecutor;
            _genreDataMapper = genreDataMapper;
            _tvShowDataMapper = tvShowDataMapper;
            _tvShowGenreDataMapper = tvShowGenreDataMapper;
        }

        public async Task AddTvShowsAsync(long latestTvShowIdInDatabase, TvShow[] shows)
        {
            ValidateLatestTvShowId(latestTvShowIdInDatabase);

            var addedTvShows = await AddTvShowsToDatabaseAsync(latestTvShowIdInDatabase, shows)
                .ConfigureAwait(false);

            await AddTvShowGenresToDatabaseAsync(addedTvShows, shows)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Genre>> GetOrCreateGenresIfNotExistAsync(IEnumerable<string> genreNames)
        {
            var allGenresInDatabase = _statementExecutor
                .ExecuteQuery($@"SELECT DISTINCT Name FROM {GenreTableConfiguration.Name}")
                .Select(x => x["Name"]?.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var distinctGenreNames = genreNames.Distinct();
            var genresToBeAdded = distinctGenreNames
                .Where(x => !allGenresInDatabase.Any(y => y == x));

            var objectsToAdd = genresToBeAdded.Select(x => new Dictionary<string, object>
            {
                ["Name"] = x
            });

            if (objectsToAdd.Any())
                _statementExecutor.ExecuteBulkInsertOperation(GenreTableConfiguration.Name, objectsToAdd);

            var genres = _statementExecutor.ExecuteQuery($@"
                SELECT DISTINCT Id, Name
                FROM {GenreTableConfiguration.Name}
                WHERE Name IN ({string.Join(", ", distinctGenreNames.Select(x => $"'{x}'"))})
            ");

            return await Task.FromResult(_genreDataMapper.Map(genres));
        }

        private async Task<TvShow[]> AddTvShowsToDatabaseAsync(long latestTvShowIdInDatabase, TvShow[] shows)
        {
            var tvShowObjectsToAdd = _tvShowDataMapper
                .MapBack(shows)
                .ToArray();

            for (var i = 0; i < tvShowObjectsToAdd.Length; i++)
                tvShowObjectsToAdd[i]["ManuallyCreated"] = false;

            _statementExecutor.ExecuteBulkInsertOperation(TvShowTableConfiguration.Name, tvShowObjectsToAdd);

            Thread.Sleep(TimeSpan.FromSeconds(5));
            _logger.LogInformation("Saved the tv shows to the database.");

            var addedTvShowValues = _statementExecutor.ExecuteQuery($@"
                SELECT Id, Name, PremieredOn FROM {TvShowTableConfiguration.Name}
                WHERE OriginId > @OriginId
            ", new Dictionary<string, object> { ["OriginId"] = latestTvShowIdInDatabase });

            var result = _tvShowDataMapper
                .Map(addedTvShowValues)
                .ToArray();

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        private async Task AddTvShowGenresToDatabaseAsync(TvShow[] addedTvShows, TvShow[] showsWithGenres)
        {
            for (var i = 0; i < addedTvShows.Length; i++)
            {
                var show = addedTvShows[i];
                var tvShowWithGenre = showsWithGenres
                    .FirstOrDefault(x => x.Name == show.Name && x.PremieredOn.IsDateEqualTo(show.PremieredOn));

                if (tvShowWithGenre is null || tvShowWithGenre.Genres is null || !tvShowWithGenre.Genres.Any())
                    continue;

                addedTvShows[i].Genres = tvShowWithGenre.Genres.Select(x => new TvShowGenre
                {
                    GenreId = x.GenreId,
                    TvShowId = show.Id
                });
            }

            var tvShowGenres = addedTvShows
                .Where(x => x.Genres != null && x.Genres.Any())
                .SelectMany(x => x.Genres)
                .ToArray();

            _logger.LogInformation("Adding the tv show - genre relations");

            var objects = _tvShowGenreDataMapper.MapBack(tvShowGenres);
            _statementExecutor.ExecuteBulkInsertOperation(TvShowGenreTableConfiguration.Name, objects);

            _logger.LogInformation("Saved the tv show - genre relations");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private void ValidateLatestTvShowId(long id)
        {
            if (id > 0)
            {
                var latestTvShowExists = _statementExecutor.ExecuteScalar<bool>($@"
                    IF(EXISTS(SELECT 1 FROM {TvShowTableConfiguration.Name} WHERE OriginId = @Id))
                        BEGIN
                            SELECT CAST(1 AS BIT)
                        END
                    ELSE
                        BEGIN
                            SELECT CAST(0 AS BIT)
                        END
                ", new Dictionary<string, object> { ["Id"] = id });

                if (!latestTvShowExists)
                    throw new InvalidOperationException($"The latest tv show with id '{id}' is not found in the database. Make sure you provide the id of the tv show " +
                        $"that was added to the database last.");
            }
        }
    }
}