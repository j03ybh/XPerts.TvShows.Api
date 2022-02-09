using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Repositories;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories
{
    public sealed class TvShowGenreRepository : JoinedRepository<TvShowGenre>
    {
        public TvShowGenreRepository(IStatementExecutor executor, IModelMapper<TvShowGenre> mapper, IStatementConstructor<TvShowGenre> statementBuilder)
            : base(executor, mapper, statementBuilder)
        {
        }
    }
}