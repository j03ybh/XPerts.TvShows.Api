using TechMinimalists.Database.Sql;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.StatementConstructors
{
    public sealed class TvShowGenreStatementConstructor : SqlJoinedTableStatementConstructor<TvShowGenre>
    {
        public TvShowGenreStatementConstructor()
            : base(nameof(TvShowGenre.TvShowId), nameof(TvShowGenre.GenreId), TvShowTableConfiguration.Name, GenreTableConfiguration.Name)
        {
        }

        protected override string TableName => TvShowGenreTableConfiguration.Name;
    }
}