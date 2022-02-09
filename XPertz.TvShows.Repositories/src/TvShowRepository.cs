using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Repositories;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories
{
    public sealed class TvShowRepository : ReadWriteRepository<TvShow>
    {
        public TvShowRepository(IStatementExecutor executor, IModelMapper<TvShow> mapper, IStatementConstructor<TvShow> statementBuilder)
            : base(executor, mapper, statementBuilder)
        {
        }
    }
}