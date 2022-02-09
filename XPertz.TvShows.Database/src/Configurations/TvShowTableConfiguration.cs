using TechMinimalists.Database.Sql.Configuration;

namespace XPerts.TvShows.Database
{
    public sealed class TvShowTableConfiguration : SqlTableConfiguration
    {
        public const string Name = "TV.TvShows";

        public override string TableName => Name;

        protected override string ColumnConfiguration =>
        @"
            Id bigint NOT NULL IDENTITY(1,1) PRIMARY KEY NONCLUSTERED,
            Name varchar(255) NOT NULL,
            PremieredOn date NULL,
            ManuallyCreated bit NOT NULL DEFAULT(1),
            OriginId bigint NULL,
            INDEX premieredOn_index_tvshow CLUSTERED (PremieredOn DESC)
        ";
    }
}