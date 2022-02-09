using System.Collections.Generic;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Repositories.UnitTests.Utilities
{
    public static class ScriptsExecution
    {
        public static void PrePopulateGenreTable(IStatementExecutor sqlStatementExecutor)
        {
            var insertStatement = Scripts.GetGenreInsertTestData();
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static void PrePopulateGenreTable(string[] genreNames, IStatementExecutor sqlStatementExecutor)
        {
            var insertStatement = Scripts.GenreInsertTestData_WithCustomNames(genreNames);
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static void PrePopulateTvShowTable(IStatementExecutor sqlStatementExecutor)
        {
            var insertStatement = Scripts.GetTvShowInsertTestData();
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static void PrePopulateTvShowGenreTable(IStatementExecutor sqlStatementExecutor, IDictionary<long, long> tvShow_Genre_Relations)
        {
            var insertStatement = Scripts.TvShowGenreInsertTestData(tvShow_Genre_Relations);
            sqlStatementExecutor.ExecuteNonQuery(insertStatement);
        }

        public static IEnumerable<Genre> GetGenresFromTable(IStatementExecutor sqlStatementExecutor, IModelMapper<Genre> mapper)
        {
            var selectStatement = Scripts.GenreGetTestData;
            var result = sqlStatementExecutor.ExecuteQuery(selectStatement);
            return mapper.Map(result);
        }

        public static IEnumerable<TvShowGenre> GetTvShowGenresFromTable(IStatementExecutor sqlStatementExecutor, IModelMapper<TvShowGenre> mapper)
        {
            var selectStatement = Scripts.TvShowGenreGetTestData;
            var result = sqlStatementExecutor.ExecuteQuery(selectStatement);
            return mapper.Map(result);
        }

        public static IEnumerable<TvShow> GetTvShowsFromTable(IStatementExecutor sqlStatementExecutor, IModelMapper<TvShow> mapper)
        {
            var selectStatement = Scripts.TvShowGetTestData;
            var result = sqlStatementExecutor.ExecuteQuery(selectStatement);
            return mapper.Map(result);
        }

        public static void CleanUpTvShowGenreTestData(IStatementExecutor sqlStatementExecutor)
        {
            var deleteStatement = Scripts.TvShowGenreDeleteTestData;
            sqlStatementExecutor.ExecuteNonQuery(deleteStatement);
        }

        public static void CleanUpTvShowTestData(IStatementExecutor sqlStatementExecutor)
        {
            var deleteStatement = Scripts.TvShowDeleteTestData;
            sqlStatementExecutor.ExecuteNonQuery(deleteStatement);
        }

        public static void CleanUpGenreTestData(IStatementExecutor sqlStatementExecutor)
        {
            var deleteStatement = Scripts.GenreDeleteTestData;
            sqlStatementExecutor.ExecuteNonQuery(deleteStatement);
        }
    }
}