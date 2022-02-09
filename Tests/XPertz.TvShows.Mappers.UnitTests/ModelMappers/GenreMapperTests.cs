using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Core.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Mappers.UnitTests
{
    public class GenreMapperTests
    {
        private static IModelMapper<Genre> GetTestSubject() => new GenreMapper();

        [Test]
        public void WhenMapDictionary_ThenReturnsModel()
        {
            // Arrange
            var dictionary = BuildRandomGenreDictionary();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(dictionary[nameof(Genre.Id)], result.Id);
            Assert.AreEqual(dictionary[nameof(Genre.Name)], result.Name);
        }

        [Test]
        public void WhenMapDictionaries_ThenReturnsModels()
        {
            // Arrange
            var dictionaries = BuildRandomGenreDictionaries(5).ToArray();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionaries).ToArray();

            // Assert
            Assert.NotNull(result);
            Assert.Greater(result.Length, 0);
            Assert.AreEqual(dictionaries.Length, result.Length);

            for (var i = 0; i < result.Length; i++)
            {
                var dictionary = dictionaries[i];
                var model = result[i];

                Assert.AreEqual(dictionary[nameof(Genre.Id)], model.Id);
                Assert.AreEqual(dictionary[nameof(Genre.Name)], model.Name);
            }
        }

        [Test]
        public void WhenMapBackGenre_ThenReturnsDictionary()
        {
            // Arrange
            var genre = BuildRandomGenre();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(genre);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result[nameof(Genre.Id)], genre.Id);
            Assert.AreEqual(result[nameof(Genre.Name)], genre.Name);
        }

        [Test]
        public void WhenMapBackGenres_ThenReturnsDictionaries()
        {
            // Arrange
            var genres = BuildRandomGenres(5).ToArray();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(genres).ToArray();

            // Assert
            Assert.NotNull(result);
            Assert.Greater(result.Length, 0);
            Assert.AreEqual(genres.Length, result.Length);

            for (var i = 0; i < result.Length; i++)
            {
                var genre = genres[i];
                var dictionary = result[i];

                Assert.AreEqual(dictionary[nameof(Genre.Id)], genre.Id);
                Assert.AreEqual(dictionary[nameof(Genre.Name)], genre.Name);
            }
        }

        private static IEnumerable<Genre> BuildRandomGenres(int count)
        {
            for (var i = 0; i < count; i++)
                yield return BuildRandomGenre();
        }

        private static Genre BuildRandomGenre()
        {
            return new Genre
            {
                Name = RandomValue.String(12),
                Id = RandomValue.Number(255)
            };
        }

        private static IEnumerable<IDictionary<string, object>> BuildRandomGenreDictionaries(int count)
        {
            for (var i = 0; i < count; i++)
                yield return BuildRandomGenreDictionary();
        }

        private static IDictionary<string, object> BuildRandomGenreDictionary()
        {
            return new Dictionary<string, object>
            {
                [nameof(Genre.Name)] = RandomValue.String(12),
                [nameof(Genre.Id)] = RandomValue.Number(255)
            };
        }
    }
}