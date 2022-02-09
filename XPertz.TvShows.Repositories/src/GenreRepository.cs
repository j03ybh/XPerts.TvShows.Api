using System.Data.SqlClient;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Repositories;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories
{
    public sealed class GenreRepository : ReadWriteRepository<SqlConnection, Genre>
    {
        public GenreRepository(ISqlStatementExecutor executor, IModelMapper<Genre> mapper, IStatementConstructor<Genre> statementBuilder)
            : base(executor, mapper, statementBuilder)
        {
        }
    }
}