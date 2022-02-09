using TechMinimalists.Database.Sql;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.StatementConstructors
{
    public sealed class GenreStatementConstructor : SqlStatementConstructor<Genre>
    {
        public GenreStatementConstructor()
        {
        }

        protected override string TableName => GenreTableConfiguration.Name;

        protected override string[] InsertColumnNames => new string[]
        {
            nameof(Genre.Name)
        };
    }
}