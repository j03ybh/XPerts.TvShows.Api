using System.Collections.Generic;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories.UnitTests.Utilities
{
    public static class ScriptsExecution
    {
        public static void PrePopulateGenreTable(ISqlStatementExecutor sqlStatementExecutor)
        {
            var insertStatement = Scripts.GetGenreInsertTestData();
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static void PrePopulateGenreTable(string[] genreNames, ISqlStatementExecutor sqlStatementExecutor)
        {
            var insertStatement = Scripts.GenreInsertTestData_WithCustomNames(genreNames);
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static void PrePopulateTvShowTable(ISqlStatementExecutor sqlStatementExecutor)
        {
            var insertStatement = Scripts.GetTvShowInsertTestData();
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static void PrePopulateTvShowGenreTable(ISqlStatementExecutor sqlStatementExecutor, IDictionary<long, long> tvShow_Genre_Relations)
        {
            var insertStatement = Scripts.TvShowGenreInsertTestData(tvShow_Genre_Relations);
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static IEnumerable<Genre> GetGenresFromTable(ISqlStatementExecutor sqlStatementExecutor, IModelMapper<Genre> mapper)
        {
            var selectStatement = Scripts.GenreGetTestData;
            var result = sqlStatementExecutor.ExecuteQuery(selectStatement);
            return mapper.Map(result);
        }

        public static IEnumerable<TvShowGenre> GetTvShowGenresFromTable(ISqlStatementExecutor sqlStatementExecutor, IModelMapper<TvShowGenre> mapper)
        {
            var selectStatement = Scripts.TvShowGenreGetTestData;
            var result = sqlStatementExecutor.ExecuteQuery(selectStatement);
            return mapper.Map(result);
        }

        public static IEnumerable<TvShow> GetTvShowsFromTable(ISqlStatementExecutor sqlStatementExecutor, IModelMapper<TvShow> mapper)
        {
            var selectStatement = Scripts.TvShowGetTestData;
            var result = sqlStatementExecutor.ExecuteQuery(selectStatement);
            return mapper.Map(result);
        }

        public static void CleanUpTvShowGenreTestData(ISqlStatementExecutor sqlStatementExecutor)
        {
            var deleteStatement = Scripts.TvShowGenreDeleteTestData;
            sqlStatementExecutor.ExecuteNonQuery(deleteStatement);
        }

        public static void CleanUpTvShowTestData(ISqlStatementExecutor sqlStatementExecutor)
        {
            var deleteStatement = Scripts.TvShowDeleteTestData;
            sqlStatementExecutor.ExecuteNonQuery(deleteStatement);
        }

        public static void CleanUpGenreTestData(ISqlStatementExecutor sqlStatementExecutor)
        {
            var deleteStatement = Scripts.GenreDeleteTestData;
            sqlStatementExecutor.ExecuteNonQuery(deleteStatement);
        }
    }
}