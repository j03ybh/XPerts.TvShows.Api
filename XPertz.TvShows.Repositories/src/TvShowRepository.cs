using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories
{
    public sealed class TvShowRepository : SqlReadWriteRepository<TvShow>
    {
        public TvShowRepository(ISqlStatementExecutor executor, IModelMapper<TvShow> mapper, IStatementConstructor<TvShow> statementBuilder)
            : base(executor, mapper, statementBuilder)
        {
        }
    }
}