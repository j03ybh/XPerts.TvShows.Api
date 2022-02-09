using TechMinimalists.Database.Sql.Configuration;

namespace XPerts.TvShows.Database
{
    public sealed class GenreTableConfiguration : SqlTableConfiguration
    {
        public const string Name = "TV.Genres";

        public override string TableName => Name;

        protected override string ColumnConfiguration =>
        @"
            Id bigint NOT NULL IDENTITY(1,1) PRIMARY KEY,
            Name varchar(255) NOT NULL,
            INDEX genre_name_index (Name)
        ";
    }
}