using System.Net;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Repositories;
using TechMinimalists.Mapping;
using XPerts.TvShows.Models;

namespace XPertz.TvShows.Controllers
{
    /// <summary>
    /// Represents the data controller that uses repositories to read and update data in the database, and maps the raw data back to view model which the API controller can
    /// send back to the user.
    /// </summary>
    /// <seealso cref="XPertz.TvShows.Controllers.IDataController&lt;XPerts.TvShows.Models.TvShow, XPerts.TvShows.Models.TvShowView&gt;" />
    public class TvShowDataController : IDataController<TvShow, TvShowView>
    {
        private readonly IReadWriteRepository<TvShow> _tvShowRepository;
        private readonly IReadWriteRepository<Genre> _genreRepository;
        private readonly IJoinedRepository<TvShowGenre> _tvShowGenreRepository;
        private readonly IModelViewMapper<TvShow, TvShowView> _viewMapper;
        private readonly IPageCollection<TvShowView> _pageCollection;

        public TvShowDataController(
            IReadWriteRepository<TvShow> tvShowRepository,
            IReadWriteRepository<Genre> genreRepository,
            IJoinedRepository<TvShowGenre> tvShowGenreRepository,
            IModelViewMapper<TvShow, TvShowView> viewMapper,
            IPageCollection<TvShowView> pageCollection)
        {
            _tvShowRepository = tvShowRepository;
            _genreRepository = genreRepository;
            _tvShowGenreRepository = tvShowGenreRepository;
            _viewMapper = viewMapper;
            _pageCollection = pageCollection;
        }

        public async Task<TvShowView> AddAsync(TvShowView modelView, CancellationToken cancellationToken = default)
        {
            var model = _viewMapper.MapBack(modelView);
            var tvShow = await _tvShowRepository
                .AddAsync(model, cancellationToken)
                .ConfigureAwait(false);

            if (modelView.Genres is not null && modelView.Genres.Any())
            {
                var genres = await GetOrCreateGenresIfNotExistsAsync(modelView.Genres)
                    .ConfigureAwait(false);

                await AddGenresForTvShowAsync(genres, tvShow.Id)
                    .ConfigureAwait(false);

                await AssignGenresToTvShowAsync(tvShow)
                    .ConfigureAwait(false);
            }

            if (tvShow.IndexPosition > 0)
                _pageCollection.RefreshPage(tvShow.IndexPosition);

            return _viewMapper.Map(tvShow);
        }

        public async Task AddAsync(IEnumerable<TvShowView> modelViews, CancellationToken cancellationToken = default)
        {
            var models = _viewMapper.MapBack(modelViews);

            await _tvShowRepository
                .AddAsync(models.ToArray(), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<TvShowView> GetAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var model = await _tvShowRepository
                    .GetAsync(id, cancellationToken)
                    .ConfigureAwait(false);

                await AssignGenresToTvShowAsync(model).ConfigureAwait(false);

                return _viewMapper.Map(model);
            }
            catch (EntityNotFoundException e)
            {
                throw new ExceptionResult(HttpStatusCode.NotFound, $"Object with id '{id}' does not exist.", e);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An unexpected error occurred while trying to get an entity by id. See inner exception for details.", e);
            }
        }

        public async Task<IPage<TvShowView>> GetAsync(int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            if (pageNumber == 0)
                throw new ExceptionResult(HttpStatusCode.BadRequest, "The page number must be greater or equal to 1.");

            var page = await _pageCollection.GetOrCachePageAsync(pageNumber, async (indexQuery) =>
            {
                var models = await _tvShowRepository
                    .QueryAsync(indexQuery)
                    .ConfigureAwait(false);

                var modelsArray = models.ToArray();
                for (var i = 0; i < modelsArray.Length; i++)
                {
                    await AssignGenresToTvShowAsync(modelsArray[i])
                        .ConfigureAwait(false);
                }

                return _viewMapper.Map(modelsArray);
            })
            .ConfigureAwait(false);

            return page;
        }

        public async Task<IEnumerable<TvShowView>> QueryAsync(ColumnQuery[] columnQueries, CancellationToken cancellationToken = default)
        {
            var models = await _tvShowRepository
                .QueryAsync(columnQueries)
                .ConfigureAwait(false);

            var modelsArray = models.ToArray();
            for (var i = 0; i < modelsArray.Length; i++)
            {
                await AssignGenresToTvShowAsync(modelsArray[i])
                    .ConfigureAwait(false);
            }

            return _viewMapper.Map(modelsArray);
        }

        public async Task<TvShowView> UpdateAsync(TvShowView modelView, CancellationToken cancellationToken = default)
        {
            var show = await _tvShowRepository
                    .GetAsync(modelView.Id, cancellationToken)
                    .ConfigureAwait(false);

            AssignPropertiesToUpdate(show, modelView);
            var updatedShow = await _tvShowRepository
                .UpdateAsync(show, cancellationToken)
                .ConfigureAwait(false);

            if (modelView.Genres is not null && modelView.Genres.Any())
            {
                var genres = await GetOrCreateGenresIfNotExistsAsync(modelView.Genres)
                    .ConfigureAwait(false);

                await UpdateGenresForTvShowAsync(genres, updatedShow.Id)
                    .ConfigureAwait(false);
            }

            await AssignGenresToTvShowAsync(updatedShow)
                .ConfigureAwait(false);

            if (updatedShow.IndexPosition > 0)
                _pageCollection.RefreshPage(updatedShow.IndexPosition);

            return _viewMapper.Map(updatedShow);
        }

        public async Task DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            var tvShow = await _tvShowRepository
                .GetAsync(id, cancellationToken)
                .ConfigureAwait(false);

            await _tvShowGenreRepository.DeleteAsync(new ColumnQuery[]
            {
                new ColumnQuery(nameof(TvShowGenre.TvShowId), id)
            })
            .ConfigureAwait(false);

            await _tvShowRepository
                .DeleteAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (tvShow.IndexPosition > 0)
                _pageCollection.RefreshPage(tvShow.IndexPosition);
        }

        private void AssignPropertiesToUpdate(TvShow show, TvShowView view)
        {
            if (show is not null && view is not null)
            {
                if (!string.IsNullOrWhiteSpace(view.PremieredOn))
                    show.PremieredOn = DateTime.TryParse(view.PremieredOn, out var premieredOn)
                        ? premieredOn
                        : throw new ExceptionResult(HttpStatusCode.BadRequest, "Please provide a valid date in the format yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(view.Name))
                    show.Name = view.Name;
            }
        }

        private async Task AssignGenresToTvShowAsync(TvShow model)
        {
            if (model is not null)
            {
                var columnQueries = new ColumnQuery[]
                {
                    new ColumnQuery(nameof(TvShowGenre.TvShowId), model.Id)
                };

                var modelHasGenres = await _tvShowGenreRepository
                    .ExistsAsync(columnQueries)
                    .ConfigureAwait(false);

                if (modelHasGenres)
                {
                    var tvShowGenres = await _tvShowGenreRepository
                        .QueryAsync(columnQueries)
                        .ConfigureAwait(false);

                    model.Genres = tvShowGenres;
                }
                else
                    model.Genres = Array.Empty<TvShowGenre>();
            }
        }

        private async Task<IEnumerable<Genre>> GetOrCreateGenresIfNotExistsAsync(IEnumerable<string> genreNames)
        {
            if (genreNames is not null && genreNames.Count() > 0)
            {
                var query = new ColumnQuery[]
                {
                    new ColumnQuery(nameof(Genre.Name), genreNames.ToArray(), QueryCondition.In)
                };

                var result = await _genreRepository
                    .QueryAsync(query)
                    .ConfigureAwait(false);

                if (!result.Any() || genreNames.Count() != result.Count())
                {
                    var genresToAdd = genreNames
                        .Where(x => !result.Any(y => y.Name == x))
                        .Select(x => new Genre
                        {
                            Name = x
                        })
                        .ToArray();

                    await _genreRepository
                        .AddAsync(genresToAdd)
                        .ConfigureAwait(false);

                    return await _genreRepository
                        .QueryAsync(query)
                        .ConfigureAwait(false);
                }

                return result;
            }

            return Array.Empty<Genre>();
        }

        private async Task AddGenresForTvShowAsync(IEnumerable<Genre> genres, long tvShowId)
        {
            if (genres is not null)
            {
                var tvShowGenresToAdd = genres.Select(x => new TvShowGenre
                {
                    GenreId = x.Id,
                    TvShowId = tvShowId
                })
                .ToArray();

                if (tvShowGenresToAdd.Length > 0)
                {
                    await _tvShowGenreRepository
                        .AddAsync(tvShowGenresToAdd)
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task UpdateGenresForTvShowAsync(IEnumerable<Genre> genres, long tvShowId)
        {
            var existingRelations = await _tvShowGenreRepository.QueryAsync(new ColumnQuery[]
            {
                new ColumnQuery(nameof(TvShowGenre.TvShowId), tvShowId)
            })
            .ConfigureAwait(false);

            IEnumerable<TvShowGenre> genresToDelete;
            if (genres is not null)
            {
                var genresToAdd = genres
                    .Where(x => !existingRelations.Any(y => y.GenreId == x.Id));
                genresToDelete = existingRelations
                    .Where(x => !genres.Any(y => y.Id == x.GenreId));

                foreach (var genre in genresToAdd)
                {
                    await _tvShowGenreRepository.AddAsync(new TvShowGenre
                    {
                        GenreId = genre.Id,
                        TvShowId = tvShowId
                    })
                    .ConfigureAwait(false);
                }
            }
            else
                genresToDelete = existingRelations;

            foreach (var tvShowGenre in genresToDelete)
            {
                await _tvShowGenreRepository.DeleteAsync(new ColumnQuery[]
                {
                    new ColumnQuery(nameof(TvShowGenre.TvShowId), tvShowId),
                    new ColumnQuery(nameof(TvShowGenre.GenreId), tvShowGenre.GenreId)
                })
                .ConfigureAwait(false);
            }
        }
    }
}