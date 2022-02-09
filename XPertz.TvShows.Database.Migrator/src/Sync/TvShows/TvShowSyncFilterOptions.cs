namespace XPertz.TvShows.Database.Migrator.Sync
{
    /// <summary>
    /// Represents options to configure a sync filter for tv shows data import.
    /// </summary>
    public class TvShowSyncFilterOptions
    {
        public const string OptionsName = "TvShowSyncFilter";

        public TvShowSyncFilterOptions()
        {
        }

        /// <summary>
        /// Gets or sets the earliest premiere date; used to filter only data import of tv shows that are premiered after this specified date.
        /// </summary>
        /// <value>
        /// The earliest premiere date.
        /// </value>
        public string EarliestPremiereDate { get; set; }
    }
}