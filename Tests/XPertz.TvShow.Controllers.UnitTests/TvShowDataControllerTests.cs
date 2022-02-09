using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechMinimalists.Database.Configuration;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Configuration;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Mappers;
using XPerts.TvShows.Models;
using XPerts.TvShows.StatementConstructors;
using XPertz.TvShows.Repositories;
using XPertz.TvShows.Repositories.UnitTests.Utilities;

namespace XPertz.TvShows.Controllers.UnitTests
{
    [TestFixture]
    public class TvShowDataControllerTests
    {
        private const int _pageSize = 100;
        private const int _totalRecordCount = 500;
        private static readonly GenreMapper _genreDataMapper = new();
        private static readonly TvShowMapper _tvShowDataMapper = new();
        private static readonly string _databaseName = "TestingEverything";
        private SqlTestDatabase _database;
        private TvShowDataController _testSubject;
        private IStatementExecutor _statementExecutor;
        private IMemoryCache _cache;
        private PageCollection<TvShowView> _pageCollection;

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

            _testSubject = GetTestSubject();

            for (var i = 0; i < _totalRecordCount; i++)
            {
                ScriptsExecution.PrePopulateGenreTable(_statementExecutor);
                ScriptsExecution.PrePopulateTvShowTable(_statementExecutor);
            }

            for (long i = 1; i < _totalRecordCount; i++)
            {
                ScriptsExecution.PrePopulateTvShowGenreTable(_statementExecutor, new Dictionary<long, long>
                {
                    [i] = i
                });
            }
        }

        [OneTimeTearDown]
        public void CleanUpTestClass()
        {
            _database.Drop();
        }

        [Test]
        public async Task WhenGetAsync_ThenCachesAndReturnsPageByPageNumber()
        {
            // Arramge
            ClearCache();

            // Act: Get page 1 before caching
            var page1Before = DateTime.Now;
            var page1Result = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);
            var page1After = DateTime.Now;
            var page1TimeToGetPage = page1After - page1Before;

            // Assert: Page 1 has 100 records
            Assert.NotNull(page1Result);
            Assert.AreEqual(_pageSize, page1Result.Count);

            // Act: Get page 1 after caching
            var secondTimePage1Before = DateTime.Now;
            var page1ResultSecondTime = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);
            var secondTimePage1After = DateTime.Now;
            var page1TimeToGetCachedPage = secondTimePage1After - secondTimePage1Before;

            // Assert: Page 1 after caching is same as page 1 before caching
            Assert.NotNull(page1ResultSecondTime);
            Assert.Greater(page1TimeToGetPage.Milliseconds, page1TimeToGetCachedPage.Milliseconds * 50);
            Assert.AreEqual(page1Result.Count, page1ResultSecondTime.Count);
            Assert.IsTrue(page1Result.Values.All(x => page1ResultSecondTime.Values.Any(y => y.Id == x.Id)));

            // Act: Get page 2
            var page2Result = await _testSubject
                .GetAsync(2)
                .ConfigureAwait(false);

            // Assert: Page 2 also has 100 records and has only new records
            Assert.NotNull(page2Result);
            Assert.AreEqual(_pageSize, page2Result.Count);
            Assert.IsFalse(page1Result.Values.Any(x => page2Result.Values.Any(y => y.Id == x.Id)));
        }

        [Test]
        public async Task GetByIdAsync_ThenReturnsTVShow()
        {
            // Arrange
            long id = 134;

            // Act
            var result = await _testSubject
                .GetAsync(id)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.NotNull(result.Genres);
            Assert.Greater(result.Genres.Count(), 0);
            Assert.NotNull(result.Genres.First());
            Assert.AreNotEqual(result.Genres.First(), string.Empty);
            Assert.Greater(result.Genres.First().Length, 1);
        }

        [Test]
        public async Task WhenAddAsync_AndPremieredOnIsMostRecent_ThenAddsNewSeries_And_RefreshesFirstCachedPage()
        {
            // Arrange
            ClearCache();
            var tvShow = new TvShowView
            {
                Name = "Very recent amazing series",
                PremieredOn = DateTime.Now.ToString("yyyyy-MM-dd")
            };

            // Pre-Assertion
            var firstPage = await _testSubject
                .GetAsync(1)
                .ConfigureAwait(false);
            Assert.NotNull(firstPage);
            Assert.AreNotEqual(tvShow.PremieredOn, firstPage.Values.First().PremieredOn);
            Assert.AreNotEqual(tvShow.Name, firstPage.Values.First().Name);

            // Act
            var result = await _testSubject
                .AddAsync(tvShow, default)
                .ConfigureAwait(false);

            firstPage = await _testSubject
                .GetAsync(1)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(firstPage);

            var firstRecord = firstPage.Values.First();
            Assert.AreEqual(firstRecord.Id, result.Id);
            Assert.AreEqual(firstRecord.Name, result.Name);
            Assert.AreEqual(firstRecord.Name, result.Name);
        }

        [Test, Order(1)]
        public async Task WhenAddBulkAsync_ThenAddsNewSeries()
        {
            // Arrange
            var newShows = Enumerable.Range(0, 5).Select(x => new TvShowView
            {
                Name = RandomValue.String(25),
                PremieredOn = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
            })
            .ToArray();

            // Act
            await _testSubject
                .AddAsync(newShows)
                .ConfigureAwait(false);

            // Assert
            ClearCache();
            var firstPageWithNewestSeries = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            var records = firstPageWithNewestSeries.Values.ToArray();
            for (var i = 0; i < 5; i++)
            {
                Assert.NotNull(records[i]);
                Assert.IsTrue(newShows.Any(x => x.Name == records[i].Name));
                Assert.AreEqual(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), records[i].PremieredOn);
            }
        }

        [Test]
        public async Task WhenQueryAsync_ThenGetsShowsByName()
        {
            // Arrange
            var page = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            var testRecords = page.Values.Where((x, index) => index > 30 && index < 35);

            // Act
            foreach (var testRecord in testRecords)
            {
                var queries = new ColumnQuery[]
                {
                    new ColumnQuery(nameof(TvShow.Name), testRecord.Name)
                };

                // Assert
                var result = await _testSubject
                    .QueryAsync(queries)
                    .ConfigureAwait(false);

                Assert.NotNull(result);
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(testRecord.Id, result.First().Id);
                Assert.AreEqual(testRecord.Name, result.First().Name);
            }
        }

        [Test]
        public async Task WhenUpdateAsync_ThenUpdatesTvShowName()
        {
            // Arrange
            ClearCache();
            var page = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            var newName = "New updated name, you can validate it if you want.";
            var showToUpdate = page.Values.Last();

            // Pre-Assertion
            Assert.AreNotEqual(newName, showToUpdate.Name);

            // Act
            showToUpdate.Name = newName;
            var result = await _testSubject
                .UpdateAsync(showToUpdate)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(showToUpdate.Id, result.Id);
            Assert.AreEqual(newName, result.Name);
        }

        [Test]
        public async Task WhenUpdateAsync_ThenUpdatesTvPremieredOn()
        {
            // Arrange
            ClearCache();
            var page = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            var newPremieredOn = "1991-01-03";
            var showToUpdate = page.Values.Last();

            // Pre-Assertion
            Assert.AreNotEqual(newPremieredOn, showToUpdate.PremieredOn);

            // Act
            showToUpdate.PremieredOn = newPremieredOn;
            var result = await _testSubject
                .UpdateAsync(showToUpdate)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(showToUpdate.Id, result.Id);
            Assert.AreEqual(newPremieredOn, result.PremieredOn);
        }

        [Test]
        public async Task WhenUpdateAsync_ThenReplacesGenres()
        {
            // Arrange
            ClearCache();
            var page = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            var showToUpdate = page.Values.First(x => x.Genres != null && x.Genres.Any());
            var oldGenres = showToUpdate.Genres;
            var newGenre = "Random new genre";

            // Pre-Assertion
            Assert.Greater(showToUpdate.Genres.Count(), 0);

            // Act
            showToUpdate.Genres = new string[]
            {
                newGenre
            };
            var result = await _testSubject
                .UpdateAsync(showToUpdate)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(showToUpdate.Id, result.Id);
            Assert.AreEqual(1, result.Genres.Count());
            Assert.AreEqual(newGenre, result.Genres.First());
        }

        [Test]
        public async Task WhenUpdateAsync_ThenAddsGenres()
        {
            // Arrange
            ClearCache();
            var page = await _testSubject
                .GetAsync()
                .ConfigureAwait(false);

            var showToUpdate = page.Values.First(x => x.Genres != null && !x.Genres.Any());
            var genresToSteal = page.Values.First(x => x.Genres != null && x.Genres.Any()).Genres;

            // Pre-Assertion
            Assert.AreEqual(0, showToUpdate.Genres.Count());

            // Act
            showToUpdate.Genres = genresToSteal;
            var result = await _testSubject
                .UpdateAsync(showToUpdate)
                .ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(showToUpdate.Id, result.Id);
            Assert.AreEqual(genresToSteal.Count(), result.Genres.Count());
            Assert.IsTrue(genresToSteal.All(x => result.Genres.Any(y => y == x)));
        }

        [Test]
        public async Task WhenDeleteAsync_ThenRemovesTvShow()
        {
            // Arrange
            long id = 155;
            var record = await _testSubject
                .GetAsync(id)
                .ConfigureAwait(false);

            // Act
            await _testSubject
                .DeleteAsync(record.Id)
                .ConfigureAwait(false);

            // Assert
            var exception = Assert.ThrowsAsync<ExceptionResult>(async () =>
            {
                var notExistsAnymore = await _testSubject
                .GetAsync(id)
                .ConfigureAwait(false);
            });
            Assert.NotNull(exception);
            Assert.AreEqual(HttpStatusCode.NotFound, exception.StatusCode);
            Assert.IsTrue(exception.Message.Contains("155"));
        }

        private TvShowDataController GetTestSubject()
        {
            var tvShowGenreStatementConstructor = new TvShowGenreStatementConstructor();
            var genreStatementConstructor = new GenreStatementConstructor();
            var tvShowStatementConstructor = new TvShowStatementConstructor();

            var tvShowGenreMapper = new TvShowGenreMapper(_tvShowDataMapper, _genreDataMapper);

            var tvShowGenreRepository = new TvShowGenreRepository(_statementExecutor, tvShowGenreMapper, tvShowGenreStatementConstructor);
            var genreRepository = new GenreRepository(_statementExecutor, _genreDataMapper, genreStatementConstructor);
            var tvShowRepository = new TvShowRepository(_statementExecutor, _tvShowDataMapper, tvShowStatementConstructor);

            var viewMapper = new TvShowViewMapper();

            _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            _pageCollection = new PageCollection<TvShowView>(BuildConfiguration(maxPageSize: _pageSize), _cache);

            return new TvShowDataController(tvShowRepository, genreRepository, tvShowGenreRepository, viewMapper, _pageCollection);
        }

        private static IOptions<PaginationOptions> BuildConfiguration(int maxPageSize)
        {
            return Options.Create(new PaginationOptions
            {
                MaxPageSize = maxPageSize
            });
        }

        private void ClearCache()
        {
            for (long i = 1; i < _pageSize * 3; i += _totalRecordCount)
            {
                _pageCollection.RefreshPage(i);
            }
        }
    }
}