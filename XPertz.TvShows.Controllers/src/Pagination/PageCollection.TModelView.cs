using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Controllers
{
    /// <summary>
    /// Represents a collection of pages. It provides functions to get, create, and remove pages from
    /// the <see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/>.
    /// </summary>
    /// <typeparam name="TModelView">The type of the model view.</typeparam>
    /// <seealso cref="XPertz.TvShows.Controllers.IPageCollection{TModelView}" />
    public sealed class PageCollection<TModelView> : IPageCollection<TModelView>
        where TModelView : class
    {
        private static readonly Guid _modelPageTokenId = Guid.NewGuid();
        private readonly IOptions<PaginationOptions> _options;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageCollection{TModelView}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cache">The cache.</param>
        public PageCollection(IOptions<PaginationOptions> options, IMemoryCache cache)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _cache = cache;
        }

        /// <summary>
        /// Checks if the memory cache has a page object stored under the key that is constructed with the specified <see cref="XPertz.TvShows.Controllers.PageToken"/>.
        /// If the key exists, then returns the <see cref="XPertz.TvShows.Controllers.IPage{TModelView}"/>; otherwise builds the page, and
        /// caches it into memory before returning it.
        /// </summary>
        /// <param name="pageToken">The page token.</param>
        /// <param name="pageFactory">The delegate that constructs the page for the specified page token.
        /// Two <see cref="PageIndexColumnQuery"/> objects are injected that can filter on the index positions
        /// of <see cref="XPerts.TvShows.Models.IIndexable"/> objects, where their index position is greater than or equal to the start index,
        /// and less than or equal to the end index.
        /// </param>
        /// <returns>
        /// The constructed or cached page.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pageFactory</exception>
        public async Task<IPage<TModelView>> GetOrCachePageAsync(int pageNumber, Func<PageIndexColumnQuery, Task<IEnumerable<TModelView>>> pageFactory)
        {
            if (pageFactory is null)
                throw new ArgumentNullException(nameof(pageFactory));
            if (pageNumber <= 0)
                throw new ExceptionResult(System.Net.HttpStatusCode.BadRequest, "Please provide a page number that is greater or equal to 1.");

            var pageSize = _options.Value.MaxPageSize;

            return await _cache.GetOrCreateAsync<IPage<TModelView>>(BuildCacheKey(pageNumber), async (entry) =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                long startIndex = (pageNumber - 1) * pageSize + 1;

                long endIndex = startIndex + pageSize - 1;
                var indexColumnQuery = new PageIndexColumnQuery(nameof(IIndexable.IndexPosition), startIndex, endIndex);

                var viewModels = await pageFactory(indexColumnQuery).ConfigureAwait(false);
                var viewModelsArray = viewModels.ToArray();
                if (viewModelsArray.Length > pageSize)
                    throw new IndexOutOfRangeException($"The page factory brings back more data than is expected. Please make sure the data does not exceed the configured maximum page size '{pageSize}'");

                return new Page<TModelView>(viewModelsArray, pageNumber);
            })
            .ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the page in the cache in which the data model is positioned at the specified index. The next time this page is
        /// requested from the cache it will include newly added and/or updated data models.
        /// </summary>
        /// <param name="modelPageTokenId">The model page token identifier; that is, the <see cref="System.Guid"/> that identifies
        /// the type of the data model.</param>
        /// <param name="modelIndexPosition">The model index position.</param>
        public void RefreshPage(long modelIndexPosition)
        {
            try
            {
                var pageSize = _options.Value.MaxPageSize;

                var pageNumber = 1;
                for (var i = pageSize; i <= modelIndexPosition; i += pageSize)
                {
                    if (modelIndexPosition < i)
                        break;

                    pageNumber++;
                }

                _cache.Remove(BuildCacheKey(pageNumber));
            }
            catch { }
        }

        private static string BuildCacheKey(int pageNumber) => $"{_modelPageTokenId}-{pageNumber}";
    }
}