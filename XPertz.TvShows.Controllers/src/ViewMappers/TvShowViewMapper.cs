using TechMinimalists.Mapping;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.Mappers
{
    public sealed class TvShowViewMapper : ModelViewMapper<TvShow, TvShowView>
    {
        public TvShowViewMapper()
        {
        }

        protected override TvShow MapBackCore(TvShowView view)
        {
            var date = DateTime.TryParse(view.PremieredOn, out var premieredOn)
                ? premieredOn
                : default;

            return new TvShow
            {
                Name = view.Name,
                Id = view.Id,
                PremieredOn = date,
                Genres = ParseGenres(view.Genres)
            };
        }

        protected override TvShowView MapCore(TvShow model)
        {
            return new TvShowView
            {
                Name = model.Name,
                Id = model.Id,
                PremieredOn = model.PremieredOn.ToString("yyyy-MM-dd"),
                Genres = ParseGenres(model.Genres),
            };
        }

        private IEnumerable<string> ParseGenres(IEnumerable<TvShowGenre> genres)
        {
            if (genres is not null)
            {
                return genres
                    .Select(x => x.Genre?.Name)
                    .Where(x => !string.IsNullOrWhiteSpace(x));
            }

            return Array.Empty<string>();
        }

        private IEnumerable<TvShowGenre> ParseGenres(IEnumerable<string> genres)
        {
            if (genres is not null)
            {
                return genres
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .Select(x => new TvShowGenre
                    {
                        Genre = new Genre
                        {
                            Name = x
                        }
                    });
            }

            return Array.Empty<TvShowGenre>();
        }
    }
}