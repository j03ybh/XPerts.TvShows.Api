using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Core.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;
using XPertz.TvShows.Mappers.UnitTests.Utilities;

namespace XPertz.TvShows.Mappers.UnitTests
{
    [TestFixture]
    public class TvShowMapperTests
    {
        private static IModelMapper<TvShow> GetTestSubject() => new TvShowMapper();

        [Test]
        public void WhenMapDictionary_ThenReturnsModel()
        {
            // Arrange
            var dictionary = BuildRandomTvShowDictionary();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(dictionary[nameof(TvShow.Id)], result.Id);
            Assert.AreEqual(dictionary[nameof(TvShow.Name)], result.Name);
            Assert.IsTrue(TestHelpers.AreMappedDateTimesEqual(dictionary[nameof(TvShow.PremieredOn)], result.PremieredOn));
        }

        [Test]
        public void WhenMapDictionaries_ThenReturnsModels()
        {
            // Arrange
            var dictionaries = BuildRandomTvShowDictionaries(5).ToArray();
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

                Assert.AreEqual(dictionary[nameof(TvShow.Id)], model.Id);
                Assert.AreEqual(dictionary[nameof(TvShow.Name)], model.Name);
                Assert.IsTrue(TestHelpers.AreMappedDateTimesEqual(dictionary[nameof(TvShow.PremieredOn)], model.PremieredOn));
            }
        }

        [Test]
        public void WhenMapBackTvShow_ThenReturnsDictionary()
        {
            // Arrange
            var show = BuildRandomTvShow();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(show);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result[nameof(TvShow.Id)], show.Id);
            Assert.AreEqual(result[nameof(TvShow.Name)], show.Name);
            Assert.IsTrue(TestHelpers.AreMappedDateTimesEqual(result[nameof(TvShow.PremieredOn)], show.PremieredOn));
        }

        [Test]
        public void WhenMapBackTvShows_ThenReturnsDictionaries()
        {
            // Arrange
            var tvShows = BuildRandomTvShows(5).ToArray();
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(tvShows).ToArray();

            // Assert
            Assert.NotNull(result);
            Assert.Greater(result.Length, 0);
            Assert.AreEqual(tvShows.Length, result.Length);

            for (var i = 0; i < result.Length; i++)
            {
                var show = tvShows[i];
                var dictionary = result[i];

                Assert.AreEqual(dictionary[nameof(TvShow.Id)], show.Id);
                Assert.AreEqual(dictionary[nameof(TvShow.Name)], show.Name);
                Assert.IsTrue(TestHelpers.AreMappedDateTimesEqual(dictionary[nameof(TvShow.PremieredOn)], show.PremieredOn));
            }
        }

        private static IEnumerable<TvShow> BuildRandomTvShows(int count)
        {
            for (var i = 0; i < count; i++)
                yield return BuildRandomTvShow();
        }

        private static TvShow BuildRandomTvShow()
        {
            return new TvShow
            {
                Name = RandomValue.String(12),
                Id = RandomValue.Number(255),
                PremieredOn = DateTime.Now
            };
        }

        private static IEnumerable<IDictionary<string, object>> BuildRandomTvShowDictionaries(int count)
        {
            for (var i = 0; i < count; i++)
                yield return BuildRandomTvShowDictionary();
        }

        private static IDictionary<string, object> BuildRandomTvShowDictionary()
        {
            return new Dictionary<string, object>
            {
                [nameof(TvShow.Name)] = RandomValue.String(12),
                [nameof(TvShow.Id)] = RandomValue.Number(255),
                [nameof(TvShow.PremieredOn)] = DateTime.Now,
            };
        }
    }
}