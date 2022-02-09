using System.Collections.Generic;
using System.Linq;
using TechMinimalists.Database.Core;
using XPerts.TvShows.Database;

namespace XPertz.TvShows.Repositories.UnitTests.Utilities
{
    internal static class Scripts
    {
        internal static string GetGenreInsertTestData() => $@"
            INSERT INTO {GenreTableConfiguration.Name} (Name)
            VALUES('{RandomValue.String(12)}'),
            ('{RandomValue.String(12)}'),
            ('{RandomValue.String(12)}')
        ";

        internal static string GetTvShowInsertTestData() => $@"
            INSERT INTO {TvShowTableConfiguration.Name} (Name, PremieredOn)
            VALUES('{RandomValue.String(12)}', '{RandomValue.Number(1994, 2021)}-{RandomValue.Number(1, 12)}-{RandomValue.Number(1, 28)}'),
            ('{RandomValue.String(12)}', '{RandomValue.Number(1994, 2021)}-{RandomValue.Number(1, 12):00}-{RandomValue.Number(1, 28):00}'),
            ('{RandomValue.String(12)}', '{RandomValue.Number(1994, 2021)}-{RandomValue.Number(1, 12):00}-{RandomValue.Number(1, 28):00}')
        ";

        internal static string GenreInsertTestData_WithCustomNames(string[] names)
        {
            var statement = $@"
                INSERT INTO {GenreTableConfiguration.Name} (Name)
                VALUES{{0}}
            ";
            var inserts = string.Join(", ", names.Select(x => $"('{x}')"));
            return string.Format(statement, inserts);
        }

        internal static string TvShowGenreInsertTestData(IDictionary<long, long> tvShowGenreRelations)
        {
            var statement = $@"
                INSERT INTO {TvShowGenreTableConfiguration.Name} (TvShowId, GenreId)
                VALUES{{0}}
            ";
            var inserts = tvShowGenreRelations.Select(x => $"({x.Key}, {x.Value})");

            var result = string.Format(statement, string.Join(", ", inserts));
            return result;
        }

        internal static string GenreGetTestData = $@"
            SELECT * FROM {GenreTableConfiguration.Name}
        ";

        internal static string TvShowGenreGetTestData = $@"
            SELECT * FROM {TvShowGenreTableConfiguration.Name}
        ";

        internal static string TvShowGetTestData = $@"
            SELECT * FROM {TvShowTableConfiguration.Name}
        ";

        internal static string TvShowGenreDeleteTestData = $@"
            DELETE FROM {TvShowGenreTableConfiguration.Name}
        ";

        internal static string TvShowDeleteTestData = $@"
            DELETE FROM {TvShowTableConfiguration.Name}
        ";

        internal static string GenreDeleteTestData = $@"
            DELETE FROM {GenreTableConfiguration.Name}
        ";
    }
}