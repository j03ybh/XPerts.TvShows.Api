using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Repositories;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories
{
    public sealed class GenreRepository : ReadWriteRepository<Genre>
    {
        public GenreRepository(IStatementExecutor executor, IModelMapper<Genre> mapper, IStatementConstructor<Genre> statementBuilder)
            : base(executor, mapper, statementBuilder)
        {
        }
    }
}