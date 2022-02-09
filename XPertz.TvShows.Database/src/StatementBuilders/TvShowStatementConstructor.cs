using TechMinimalists.Database.Sql;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.StatementConstructors
{
    public sealed class TvShowStatementConstructor : SqlStatementConstructor<TvShow>
    {
        public TvShowStatementConstructor()
        {
        }

        protected override string TableName => TvShowTableConfiguration.Name;

        protected override string[] InsertColumnNames => new string[]
        {
            nameof(TvShow.Name),
            nameof(TvShow.PremieredOn)
        };

        protected override string GetStatement => $@"
            WITH IndexedRows AS(
                SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS IndexPosition,
                    {PrimaryKeyColumnName},
                    {nameof(TvShow.Name)},
                    {nameof(TvShow.PremieredOn)}
                FROM {TableName}
            )
            SELECT * FROM IndexedRows
        ";
    }
}