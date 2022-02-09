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
    public class TvShowGenreMapperTests
    {
        private static readonly IModelMapper<Genre> _genreMapper = new GenreMapper();
        private static readonly IModelMapper<TvShow> _tvShowMapper = new TvShowMapper();

        private static IModelMapper<TvShowGenre> GetTestSubject() => new TvShowGenreMapper(_tvShowMapper, _genreMapper);

        // _______________________________________________________ MAP _______________________________________________________

        [Test]
        public void WhenMapFromDictionary_AndBothIdsAreSpecified_ThenReturnsModelWithBothIds()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                [TvShowGenreTableConfiguration.GenreId] = 1,
                [TvShowGenreTableConfiguration.TvShowId] = 12
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.GenreId], result.GenreId);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.TvShowId], result.TvShowId);
        }

        [Test]
        public void WhenMapFromDictionary_AndGenreIdIsNoValidNumber_ThenReturnsModelWithOnlyTvShowIdAssigned()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                [TvShowGenreTableConfiguration.GenreId] = RandomValue.String(13),
                [TvShowGenreTableConfiguration.TvShowId] = 12
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.GenreId);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.TvShowId], result.TvShowId);
        }

        [Test]
        public void WhenMapFromDictionary_AndTvShowIdIsNoValidNumber_ThenReturnsModelWithOnlyGenreIdAssigned()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                [TvShowGenreTableConfiguration.GenreId] = 44,
                [TvShowGenreTableConfiguration.TvShowId] = RandomValue.String(13),
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.GenreId], result.GenreId);
            Assert.AreEqual(0, result.TvShowId);
        }

        [Test]
        public void WhenMapFromDictionary_AndGenreIdIsNotSpecified_ThenReturnsModelWithOnlyTvShowIdAssigned()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                [TvShowGenreTableConfiguration.TvShowId] = 12
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.GenreId);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.TvShowId], result.TvShowId);
        }

        [Test]
        public void WhenMapFromDictionary_AndTvShowIdIsNotSpecified_ThenReturnsModelWithOnlyGenreIdAssigned()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                [TvShowGenreTableConfiguration.GenreId] = 44
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.GenreId], result.GenreId);
            Assert.AreEqual(0, result.TvShowId);
        }

        [Test]
        public void WhenMapFromDictionary_AndTvShowPlusTvShowIdAreSpecified_ThenReturnsModelWithTvShowIdAndTvShowAssigned()
        {
            // Arrange
            var tvShow = new TvShow
            {
                Id = 5,
                Name = RandomValue.String(34),
                PremieredOn = DateTime.Now
            };
            var dictionary = _tvShowMapper.MapBack(tvShow);
            dictionary[TvShowGenreTableConfiguration.TvShowId] = tvShow.Id;
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Genre);
            Assert.AreEqual(0, result.GenreId);
            Assert.NotNull(result.TvShow);
            Assert.AreEqual(result.TvShowId, result.TvShow.Id);
            Assert.AreEqual(tvShow.Id, result.TvShow.Id);
            Assert.AreEqual(tvShow.Name, result.TvShow.Name);
            Assert.IsTrue(TestHelpers.AreMappedDateTimesEqual(tvShow.PremieredOn, result.TvShow.PremieredOn));
        }

        [Test]
        public void WhenMapFromDictionary_AndGenrePlusGenreIdAreSpecified_ThenReturnsModelWithGenreIdAndGenreAssigned()
        {
            // Arrange
            var genre = new Genre
            {
                Id = 5,
                Name = RandomValue.String(34)
            };
            var dictionary = _genreMapper.MapBack(genre);
            dictionary[TvShowGenreTableConfiguration.GenreId] = genre.Id;
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.TvShow);
            Assert.AreEqual(0, result.TvShowId);
            Assert.NotNull(result.Genre);
            Assert.AreEqual(result.GenreId, result.Genre.Id);
            Assert.AreEqual(genre.Id, result.Genre.Id);
            Assert.AreEqual(genre.Name, result.Genre.Name);
        }

        [Test]
        public void WhenMapFromDictionary_AndBothIdsAreSpecified_ThenReturnsModelWithOnlyIdsAssigned()
        {
            // Arrange
            var genre = new Genre
            {
                Id = 5,
                Name = RandomValue.String(34)
            };
            var dictionary = _genreMapper.MapBack(genre);
            dictionary[TvShowGenreTableConfiguration.GenreId] = genre.Id;
            dictionary[TvShowGenreTableConfiguration.TvShowId] = 66;
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.Map(dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.TvShow);
            Assert.Null(result.Genre);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.TvShowId], result.TvShowId);
            Assert.AreEqual(dictionary[TvShowGenreTableConfiguration.GenreId], result.GenreId);
        }

        // _______________________________________________________ MAP BACK _______________________________________________________

        [Test]
        public void WhenMapBackFromModel_AndBothIdsAreSpecified_ThenReturnsDictionaryWithIdsAssigned()
        {
            // Arrange
            var model = new TvShowGenre
            {
                GenreId = 23,
                TvShowId = 44
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(model);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(model.GenreId, result[TvShowGenreTableConfiguration.GenreId]);
            Assert.AreEqual(model.TvShowId, result[TvShowGenreTableConfiguration.TvShowId]);
        }

        [Test]
        public void WhenMapBackFromModel_AndOnlyGenreIdIsSpecified_ThenReturnsDictionaryWithOnlyGenreIdAssigned()
        {
            // Arrange
            var model = new TvShowGenre
            {
                GenreId = 23
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(model);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(model.GenreId, result[TvShowGenreTableConfiguration.GenreId]);
            Assert.IsFalse(result.ContainsKey(TvShowGenreTableConfiguration.TvShowId));
        }

        [Test]
        public void WhenMapBackFromModel_AndOnlyTvShowIdIsSpecified_ThenReturnsDictionaryWithOnlyTvShowIdAssigned()
        {
            // Arrange
            var model = new TvShowGenre
            {
                TvShowId = 23
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(model);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(model.TvShowId, result[TvShowGenreTableConfiguration.TvShowId]);
            Assert.IsFalse(result.ContainsKey(TvShowGenreTableConfiguration.GenreId));
        }

        [Test]
        public void WhenMapBackFromModels_AndBothIdsAreSpecified_ThenReturnsDictionaries()
        {
            // Arrange
            var models = new TvShowGenre[]
            {
                new TvShowGenre
                {
                    GenreId = 23,
                    TvShowId = 44
                },
                new TvShowGenre
                {
                    GenreId = 66,
                    TvShowId = 57
                }
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(models);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            var firstDictionary = result.First();
            var firstModel = models[0];

            var secondDictionary = result.Last();
            var secondModel = models[1];

            Assert.AreEqual(2, firstDictionary.Count);
            Assert.AreEqual(firstModel.GenreId, firstDictionary[TvShowGenreTableConfiguration.GenreId]);
            Assert.AreEqual(firstModel.TvShowId, firstDictionary[TvShowGenreTableConfiguration.TvShowId]);

            Assert.AreEqual(2, secondDictionary.Count);
            Assert.AreEqual(secondModel.GenreId, secondDictionary[TvShowGenreTableConfiguration.GenreId]);
            Assert.AreEqual(secondModel.TvShowId, secondDictionary[TvShowGenreTableConfiguration.TvShowId]);
        }

        [Test]
        public void WhenMapBackFromModels_AndForOneDictionaryOnlyGenreIdIsSpecified_AndForOtherDictionaryBothIdsAreSpecified_ThenReturnsDictionariesAccordingly()
        {
            // Arrange
            var models = new TvShowGenre[]
            {
                new TvShowGenre
                {
                    GenreId = 23,
                    TvShowId = 44
                },
                new TvShowGenre
                {
                    GenreId = 66
                }
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(models);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            var firstDictionary = result.First();
            var firstModel = models[0];

            var secondDictionary = result.Last();
            var secondModel = models[1];

            Assert.AreEqual(2, firstDictionary.Count);
            Assert.AreEqual(firstModel.GenreId, firstDictionary[TvShowGenreTableConfiguration.GenreId]);
            Assert.AreEqual(firstModel.TvShowId, firstDictionary[TvShowGenreTableConfiguration.TvShowId]);

            Assert.AreEqual(1, secondDictionary.Count);
            Assert.AreEqual(secondModel.GenreId, secondDictionary[TvShowGenreTableConfiguration.GenreId]);
            Assert.IsFalse(secondDictionary.ContainsKey(TvShowGenreTableConfiguration.TvShowId));
        }

        [Test]
        public void WhenMapBackFromModels_AndForOneDictionaryOnlyTvShowIdIsSpecified_AndForOtherDictionaryBothIdsAreSpecified_ThenReturnsDictionariesAccordingly()
        {
            // Arrange
            var models = new TvShowGenre[]
            {
                new TvShowGenre
                {
                    GenreId = 23,
                    TvShowId = 44
                },
                new TvShowGenre
                {
                    TvShowId = 66
                }
            };
            var testSubject = GetTestSubject();

            // Act
            var result = testSubject.MapBack(models);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            var firstDictionary = result.First();
            var firstModel = models[0];

            var secondDictionary = result.Last();
            var secondModel = models[1];

            Assert.AreEqual(2, firstDictionary.Count);
            Assert.AreEqual(firstModel.GenreId, firstDictionary[TvShowGenreTableConfiguration.GenreId]);
            Assert.AreEqual(firstModel.TvShowId, firstDictionary[TvShowGenreTableConfiguration.TvShowId]);

            Assert.AreEqual(1, secondDictionary.Count);
            Assert.AreEqual(secondModel.TvShowId, secondDictionary[TvShowGenreTableConfiguration.TvShowId]);
            Assert.IsFalse(secondDictionary.ContainsKey(TvShowGenreTableConfiguration.GenreId));
        }
    }
}