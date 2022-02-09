using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using TechMinimalists.Clients.Core;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    /// <summary>
    /// Represents a wrapper around the Maze TV Api. An <see cref="TechMinimalists.Clients.Core.IApiClient"/> is injected which in its turn is configured with injected
    /// <see cref="TechMinimalists.Clients.Core.ApiOptions"/>.
    /// </summary>
    /// <seealso cref="XPertz.TvShows.Database.Migrator.Sync.IMazeTvApiClient" />
    public class MazeTvApiClient : IMazeTvApiClient
    {
        private readonly IApiClient _apiClient;
        private const string _baseUrl = "https://api.tvmaze.com/shows";
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeTvApiClient"/> class.
        /// </summary>
        /// <param name="apiClient">The API client.</param>
        /// <param name="logger">The logger.</param>
        public MazeTvApiClient(IApiClient apiClient, ILogger<MazeTvApiClient> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets the tv shows from the Maze API asynchronously.
        /// </summary>
        /// <param name="startPage">The start page.</param>
        /// <returns>
        /// A collection of Maze TV Shows mapped to <see cref="XPerts.TvShows.Models.TvShowView"/> objects.
        /// </returns>
        public async Task<TvShowView[]> GetTvShowsAsync(int startPage)
        {
            var startTime = DateTime.Now;
            var maximumExecutionTime = TimeSpan.FromMinutes(1);

            _logger.LogInformation("Starting to retrieve latest tv shows from Maze API, starting with page {page}", startPage);

            var result = new List<TvShowView>();

            using var httpClient = GetHttpClient();
            var pageNumber = startPage;
            do
            {
                try
                {
                    var url = string.Concat(_baseUrl, "?page=", pageNumber);

                    var tvShowViews = await _apiClient
                        .ExecuteApiRequestAsync<IEnumerable<TvShowView>>(httpClient, HttpMethod.Get, url)
                        .ConfigureAwait(false);

                    if (tvShowViews is null || !tvShowViews.Any())
                        break;

                    result.AddRange(tvShowViews);
                    pageNumber++;
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                        _logger.LogError(ex, "Error when executing request");
                    break;
                }
                catch (Exception e)
                {
                    if (e.InnerException is HttpRequestException requestException)
                    {
                        if (requestException.StatusCode != System.Net.HttpStatusCode.NotFound)
                            _logger.LogError(requestException, "Error when executing request");
                        break;
                    }

                    _logger.LogError(exception: e, "Error occurred");
                    break;
                }
            }
            while ((DateTime.Now - startTime) < maximumExecutionTime);

            _logger.LogInformation(
                "Retrieved a total of {page} pages, with a total of {numberRecords} records",
                pageNumber - startPage,
                result.Count
            );

            return result.ToArray();
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}