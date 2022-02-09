namespace XPertz.TvShows.Controllers
{
    /// <summary>
    /// Represents options to configura pagination.
    /// </summary>
    public class PaginationOptions
    {
        public const string OptionsName = "Pagination";

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationOptions"/> class.
        /// </summary>
        public PaginationOptions()
        {
        }

        /// <summary>
        /// Gets or sets the maximum size of the page.
        /// </summary>
        /// <value>
        /// The maximum size of the page.
        /// </value>
        public int MaxPageSize { get; set; }
    }
}