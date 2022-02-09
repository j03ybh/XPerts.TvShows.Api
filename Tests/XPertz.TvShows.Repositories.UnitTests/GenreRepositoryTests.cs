using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMinimalists.Database.Configuration;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Configuration;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;
using XPerts.TvShows.StatementConstructors;
using XPertz.TvShows.Repositories.UnitTests.Utilities;

namespace XPertz.TvShows.Repositories.UnitTests
{
    [TestFixture, SingleThreaded]
    public class GenreRepositoryTests
    {
        private static readonly string _databaseName = "TestingEverything";
        private SqlTestDatabase _database;
        private GenreRepository _testSubject;
        private IStatementExecutor _statementExecutor;
        private GenreMapper _mapper;

        [OneTimeSetUp]
        public void InitializeTestClass()
        {
            var options = Options.Create(new DatabaseOptions
            {
                Name = _databaseName,
                DataSource = "."
            });
            var tableConfigurations = new IDatabaseTableConfiguration[]
            {
                new GenreTableConfiguration()
            };
            _database = new SqlTestDatabase(options, tableConfigurations);
            _database.AddSchemaConfiguration(new SqlSchemaConfiguration("TV"));

            _statementExecutor = _database.CreateDatabase();

            var genreStatementConstructor = new GenreStatementConstructor();
            _mapper = new GenreMapper();
            _testSubject = new GenreRepository(_statementExecutor, _mapper, genreStatementConstructor);
        }

        [OneTimeTearDown]
        public void CleanUpTestClass()
        {
            _database.Drop();
        }

        [SetUp]
        public void SetUpTest()
        {
            ScriptsExecution.CleanUpGenreTestData(_statementExecutor);
            ScriptsExecution.PrePopulateGenreTable(_statementExecutor);
        }

        [TearDown]
        public void CleanUpTest()
        {
            ScriptsExecution.CleanUpGenreTestData(_statementExecutor);
        }

        // _______________________________________________________________ ADD _______________________________________________________________

        [Test, Order(1)]
        public async Task WhenAddAsync_ThenAddsGenre()
        {
            // Arrange
            var genre = new Genre
            {
                Name = "Horror"
            };

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());
            Assert.IsFalse(allGenres.Any(x => x.Name == genre.Name));

            // Act
            var result = await _testSubject
                .AddAsync(genre)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(4, result.Id);
            Assert.AreEqual(genre.Name, result.Name);

            allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(4, allGenres.Count());
            Assert.IsTrue(allGenres.Any(x => x.Name == result.Name));
        }

        [Test]
        public void WhenAddAsync_AndGenreIsNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            Genre genre = null;

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var result = await _testSubject
                    .AddAsync(genre)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);

            allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());
        }

        [Test]
        public async Task WhenAddSilentlyAsync_ThenAddsGenre()
        {
            // Arrange
            var genre = new Genre
            {
                Name = "Some random genre that does not exist yet"
            };

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());
            Assert.IsFalse(allGenres.Any(x => x.Name == genre.Name));

            // Act
            await _testSubject
                .AddSilentlyAsync(genre)
                .ConfigureAwait(false);

            // Assert
            allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(4, allGenres.Count());
            Assert.IsTrue(allGenres.Any(x => x.Name == genre.Name));
        }

        [Test]
        public void WhenAddSilentlyAsync_AndGenreIsNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            Genre genre = null;

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _testSubject
                    .AddSilentlyAsync(genre)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);

            allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());
        }

        [Test]
        public async Task WhenAddBulkAsync_ThenAddsGenres()
        {
            // Arrange
            var genres = new Genre[]
            {
                new Genre
                {
                    Name = "Some random genre that does not exist yet"
                },
                new Genre
                {
                    Name = "Comedy"
                },
                new Genre
                {
                    Name = "Adventure"
                }
            };

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());
            Assert.IsFalse(allGenres.Any(x => genres.Any(y => y.Name == x.Name)));

            // Act
            await _testSubject
                .AddAsync(genres)
                .ConfigureAwait(false);

            // Assert
            allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(6, allGenres.Count());
            Assert.IsTrue(genres.All(x => allGenres.Any(y => y.Name == x.Name)));
        }

        [Test]
        public void WhenAddBulkAsync_AndGenreIsNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            Genre[] genres = null;

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _testSubject
                    .AddAsync(genres)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);

            allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allGenres);
            Assert.AreEqual(3, allGenres.Count());
        }

        // _______________________________________________________________ GET _______________________________________________________________

        [Test]
        public async Task WhenGetAsync_ThenReturnsGenres()
        {
            // Act
            var genres = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(genres);
            Assert.AreEqual(3, genres.Count());
        }

        [Test]
        public async Task WhenGetByIdAsync_ThenReturnsGenre()
        {
            // Arrange
            var genres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            var genre = genres.ToArray()[RandomValue.Number(0, genres.Count() - 1)];

            // Act
            var result = await _testSubject
                .GetAsync(genre.Id)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(genre);
            Assert.AreEqual(genre.Id, result.Id);
        }

        [Test]
        public void WhenGetByIdAsync_AndIdIsNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            object id = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _testSubject
                    .GetAsync(id)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);
        }

        // _______________________________________________________________ QUERY _______________________________________________________________

        [Test]
        public async Task WhenQueryAsync_ThenReturnsGenreWithMatchingName()
        {
            // Arrange
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            var name = allGenres.LastOrDefault()?.Name;
            var columnQueries = new ColumnQuery[]
            {
                new ColumnQuery(nameof(Genre.Name), name, QueryCondition.Equals)
            };

            // Pre-Assertion
            Assert.AreEqual(3, allGenres.Count());
            Assert.IsFalse(string.IsNullOrWhiteSpace(name));

            // Act
            var result = await _testSubject
                .QueryAsync(columnQueries)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(name, result.First().Name);
        }

        [Test]
        public async Task WhenQueryAsync_ThenReturnsGenreWithMatchingNames()
        {
            // Arrange
            var genreNames = new string[]
            {
                "Horror",
                "Horror Comedy",
                "Comedy"
            };
            ScriptsExecution.PrePopulateGenreTable(genreNames, _statementExecutor);
            var columnQueries = new ColumnQuery[]
            {
                new ColumnQuery(nameof(Genre.Name), "Comedy", QueryCondition.Contains)
            };

            // Pre-Assertion
            var allGenres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            Assert.AreEqual(6, allGenres.Count());
            Assert.IsTrue(genreNames.All(x => allGenres.Any(y => y.Name == x)));

            // Act
            var result = await _testSubject
                .QueryAsync(columnQueries)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.Count(x => x.Name == genreNames[1]));
            Assert.AreEqual(1, result.Count(x => x.Name == genreNames[2]));
        }

        [Test]
        public void WhenQueryAsync_AndColumnQueriesAreNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            ColumnQuery[] columnQueries = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _testSubject
                    .QueryAsync(columnQueries)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);
        }

        // _______________________________________________________________ UPDATE _______________________________________________________________

        [Test]
        public async Task WhenUpdateAsync_ThenUpdatesGenreProperty()
        {
            // Arrange
            var genres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            var genre = genres.ToArray()[RandomValue.Number(0, genres.Count() - 1)];
            var newName = "Some updated name that does not exist yet";

            // Pre-Assertion
            Assert.NotNull(genres);
            Assert.AreNotEqual(newName, genre.Name);
            Assert.IsFalse(genres.Any(x => x.Name == newName));

            // Act
            var updatedGenre = await _testSubject
                .UpdateAsync(genre.Id, new Dictionary<string, object>
                {
                    [nameof(Genre.Name)] = newName
                })
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(updatedGenre);
            Assert.AreEqual(genre.Id, updatedGenre.Id);
            Assert.AreNotEqual(genre.Name, updatedGenre.Name);
            Assert.AreEqual(newName, updatedGenre.Name);
        }

        [Test]
        public async Task WhenUpdateAsync_ThenUpdatesGenre()
        {
            // Arrange
            var genres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _mapper);
            var genre = genres.ToArray()[RandomValue.Number(0, genres.Count() - 1)];
            var newName = "Some updated name that does not exist yet";

            // Pre-Assertion
            Assert.NotNull(genres);
            Assert.AreNotEqual(newName, genre.Name);
            Assert.IsFalse(genres.Any(x => x.Name == newName));

            // Act
            var updatedGenre = await _testSubject
                .UpdateAsync(new Genre
                {
                    Name = newName,
                    Id = genre.Id
                })
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(updatedGenre);
            Assert.AreEqual(genre.Id, updatedGenre.Id);
            Assert.AreNotEqual(genre.Name, updatedGenre.Name);
            Assert.AreEqual(newName, updatedGenre.Name);
        }
    }
}