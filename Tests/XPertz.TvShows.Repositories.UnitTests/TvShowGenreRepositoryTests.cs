using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMinimalists.Database.Configuration;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Configuration;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;
using XPerts.TvShows.StatementConstructors;
using XPertz.TvShows.Repositories.UnitTests.Utilities;

namespace XPertz.TvShows.Repositories.UnitTests
{
    [TestFixture]
    public class TvShowGenreRepositoryTests
    {
        private static readonly string _databaseName = "TestingEverything";
        private SqlTestDatabase _database;
        private TvShowGenreRepository _testSubject;
        private ISqlStatementExecutor _statementExecutor;
        private TvShowGenreMapper _mapper;

        private GenreMapper _genreMapper;
        private TvShowMapper _tvShowMapper;

        [OneTimeSetUp]
        public void InitializeTestClass()
        {
            var options = Options.Create(new DatabaseOptions
            {
                Name = _databaseName,
                DataSource = "."
            });
            var configurations = new IDatabaseTableConfiguration[]
            {
                new TvShowTableConfiguration(),
                new GenreTableConfiguration(),
                new TvShowGenreTableConfiguration()
            };
            _database = new SqlTestDatabase(options, configurations);
            _database.AddSchemaConfiguration(new SqlSchemaConfiguration("TV"));

            _statementExecutor = _database.CreateDatabase();

            var tvShowGenreStatementConstructor = new TvShowGenreStatementConstructor();
            _genreMapper = new GenreMapper();
            _tvShowMapper = new TvShowMapper();
            _mapper = new TvShowGenreMapper(_tvShowMapper, _genreMapper);
            _testSubject = new TvShowGenreRepository(_statementExecutor, _mapper, tvShowGenreStatementConstructor);
        }

        [OneTimeTearDown]
        public void CleanUpTestClass()
        {
            _database.Drop();
        }

        [SetUp]
        public void SetUpTest()
        {
            ScriptsExecution.PrePopulateGenreTable(_statementExecutor);
            ScriptsExecution.PrePopulateTvShowTable(_statementExecutor);

            var genres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _genreMapper);
            var tvShows = ScriptsExecution.GetTvShowsFromTable(_statementExecutor, _tvShowMapper);

            ScriptsExecution.PrePopulateTvShowGenreTable(_statementExecutor, new Dictionary<long, long>
            {
                [tvShows.Last().Id] = genres.First().Id
            });
        }

        [TearDown]
        public void CleanUp()
        {
            ScriptsExecution.CleanUpTvShowGenreTestData(_statementExecutor);
            ScriptsExecution.CleanUpTvShowTestData(_statementExecutor);
            ScriptsExecution.CleanUpGenreTestData(_statementExecutor);
        }

        [Test]
        public async Task WhenAddAsync_ThenAddsTvShowGenre()
        {
            // Arrange
            var genres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _genreMapper);
            var tvShows = ScriptsExecution.GetTvShowsFromTable(_statementExecutor, _tvShowMapper);
            var tvShowGenre = new TvShowGenre
            {
                GenreId = genres.Last().Id,
                TvShowId = tvShows.Last().Id
            };

            // Pre-Assertion
            var allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(1, allTvShowGenres.Count());
            Assert.IsFalse(allTvShowGenres.Any(x => x.GenreId == tvShowGenre.GenreId && x.TvShowId == tvShowGenre.TvShowId));

            // Act
            await _testSubject
                .AddAsync(tvShowGenre)
                .ConfigureAwait(false);

            // Assert
            allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(2, allTvShowGenres.Count());
            Assert.IsTrue(allTvShowGenres.Any(x => x.GenreId == tvShowGenre.GenreId && x.TvShowId == tvShowGenre.TvShowId));
        }

        [Test]
        public void WhenAddAsync_AndTvShowGenreIsNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            TvShowGenre tvShowGenre = null;

            // Pre-Assertion
            var allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(1, allTvShowGenres.Count());

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _testSubject
                    .AddAsync(tvShowGenre)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);

            allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(1, allTvShowGenres.Count());
        }

        [Test]
        public async Task WhenAddBulkAsync_ThenAddsTvShowGenres()
        {
            // Arrange
            ScriptsExecution.CleanUpTvShowGenreTestData(_statementExecutor);
            var genres = ScriptsExecution.GetGenresFromTable(_statementExecutor, _genreMapper);
            var tvShows = ScriptsExecution.GetTvShowsFromTable(_statementExecutor, _tvShowMapper);
            var tvShowGenres = genres.Select(x =>
            {
                return new TvShowGenre
                {
                    GenreId = x.Id,
                    TvShowId = tvShows.First().Id
                };
            });

            // Pre-Assertion
            var allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(0, allTvShowGenres.Count());

            // Act
            await _testSubject
                .AddAsync(tvShowGenres.ToArray())
                .ConfigureAwait(false);

            // Assert
            allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(tvShowGenres.Count(), allTvShowGenres.Count());
            Assert.IsTrue(tvShowGenres.All(x => allTvShowGenres.Any(y => y.GenreId == x.GenreId && y.TvShowId == x.TvShowId)));
        }

        [Test]
        public void WhenAddBulkAsync_AndTvShowGenreIsNull_ThenThrowsArgumentNullException()
        {
            // Arrange
            TvShowGenre[] tvShowGenres = null;

            // Pre-Assertion
            var allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(1, allTvShowGenres.Count());

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _testSubject
                    .AddAsync(tvShowGenres)
                    .ConfigureAwait(false);
            });
            Assert.NotNull(exception);

            allTvShowGenres = ScriptsExecution.GetTvShowGenresFromTable(_statementExecutor, _mapper);
            Assert.NotNull(allTvShowGenres);
            Assert.AreEqual(1, allTvShowGenres.Count());
        }

        //// _______________________________________________________________ GET _______________________________________________________________

        [Test]
        public async Task WhenGetAsync_ThenReturnsTvShowGenres()
        {
            // Act
            var tvShowGenres = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(tvShowGenres);
            Assert.AreEqual(1, tvShowGenres.Count());
        }

        [Test, Order(1)]
        public async Task WhenGetByTvShowIdAsync_ThenReturnsTvShowGenres()
        {
            // Arrange
            ScriptsExecution.CleanUpTvShowGenreTestData(_statementExecutor);
            ScriptsExecution.PrePopulateTvShowGenreTable(_statementExecutor, new Dictionary<long, long>
            {
                [1] = 1
            });

            // Act
            var tvShowGenres = await _testSubject
                .QueryAsync(new TechMinimalists.Database.Core.ColumnQuery[]
                {
                    new TechMinimalists.Database.Core.ColumnQuery(nameof(TvShowGenre.TvShowId), 1)
                })
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(tvShowGenres);
            Assert.AreEqual(1, tvShowGenres.Count());
        }
    }
}