using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories
{
    public sealed class TvShowGenreRepository : SqlJoinedRepository<TvShowGenre>
    {
        public TvShowGenreRepository(ISqlStatementExecutor executor, IModelMapper<TvShowGenre> mapper, IStatementConstructor<TvShowGenre> statementBuilder)
            : base(executor, mapper, statementBuilder)
        {
        }
    }
}