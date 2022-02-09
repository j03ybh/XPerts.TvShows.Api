using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public class TvShowSyncFilter : ITvShowSyncFilter
    {
        private readonly ISqlStatementExecutor _statementExecutor;
        private readonly ILogger _logger;
        private readonly IOptions<TvShowSyncFilterOptions> _options;

        public TvShowSyncFilter(IOptions<TvShowSyncFilterOptions> options, ILogger<TvShowSyncFilter> logger, ISqlStatementExecutor statementExecutor)
        {
            _logger = logger;
            _options = options;
            _statementExecutor = statementExecutor;
        }

        public TvShowView[] Filter(IEnumerable<TvShowView> shows)
        {
            if (_options.Value is null)
                throw new ArgumentException("The tv show filter options are not defined (appsettings.json).");
            if (string.IsNullOrWhiteSpace(_options.Value.EarliestPremiereDate))
                throw new ArgumentException("The earliest premiered data is not configured in the tv show filter options (appsettings.json).");

            var filteredOnPremieredOn = FilterShowsOnPremieredDate(shows);
            var filteredCount = shows.Count() - filteredOnPremieredOn.Length;
            if (filteredCount != shows.Count())
            {
                _logger.LogInformation(
                    "Filtered {number} shows that were premiered before earliest configured premiere date {date}",
                    filteredCount,
                    _options.Value.EarliestPremiereDate
                );
            }

            var filteredOnDoubleShows = FilterDoubleShows(filteredOnPremieredOn);
            filteredCount = filteredOnDoubleShows.Length - filteredOnDoubleShows.Length;

            if (filteredCount != shows.Count())
            {
                _logger.LogInformation(
                    "Filtered {number} shows that already existed in the database",
                    filteredCount
                );
            }

            return filteredOnDoubleShows;
        }

        private TvShowView[] FilterDoubleShows(TvShowView[] shows)
        {
            var existingShows = _statementExecutor.ExecuteQuery($@"
                SELECT PremieredOn, Name FROM {TvShowTableConfiguration.Name}
            ");

            return shows
                .Where(x => !existingShows.Any(y => $"{y["Name"]}" == x.Name && $"{y["PremieredOn"]}" == x.PremieredOn))
                .ToArray();
        }

        private TvShowView[] FilterShowsOnPremieredDate(IEnumerable<TvShowView> shows)
        {
            if (DateTime.TryParse(_options.Value.EarliestPremiereDate, out var earliestPremieredOnDate))
            {
                var showsAfterEarliestPremieredDate = shows.Where(show =>
                {
                    return !string.IsNullOrWhiteSpace(show.PremieredOn)
                        && DateTime.TryParse(show.PremieredOn, out var premieredOn)
                        && premieredOn >= earliestPremieredOnDate;
                });
                return showsAfterEarliestPremieredDate.ToArray();
            }

            throw new ArgumentException($"The earliest premiered on date '{_options.Value.EarliestPremiereDate}' could not be parsed to a valid date time value.");
        }
    }
}