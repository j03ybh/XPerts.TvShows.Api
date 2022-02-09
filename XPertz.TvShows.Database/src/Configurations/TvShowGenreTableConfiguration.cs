using TechMinimalists.Database.Sql.Configuration;

namespace XPerts.TvShows.Database
{
    public sealed class TvShowGenreTableConfiguration : SqlTableConfiguration
    {
        public const string Name = "TV.TvShowGenres";
        public const string TvShowId = "TvShowId";
        public const string GenreId = "GenreId";

        public override string TableName => Name;

        protected override string ColumnConfiguration =>
        @"
            GenreId bigint NOT NULL,
            TvShowId bigint NOT NULL,
            FOREIGN KEY (GenreId) REFERENCES [TV].[Genres](Id),
            FOREIGN KEY (TvShowId) REFERENCES [TV].[TvShows](Id),
            CONSTRAINT PK_TvShowGenre PRIMARY KEY (GenreId,TvShowId)
        ";

        public override IEnumerable<string> DependsOn => new string[]
        {
            TvShowTableConfiguration.Name,
            GenreTableConfiguration.Name
        };
    }
}