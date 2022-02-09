using TechMinimalists.Mapping;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.Mappers
{
    public sealed class GenreViewMapper : ModelViewMapper<Genre, GenreView>
    {
        public GenreViewMapper()
        {
        }

        protected override Genre MapBackCore(GenreView view)
        {
            return new Genre
            {
                Name = view.Name,
                Id = view.Id
            };
        }

        protected override GenreView MapCore(Genre model)
        {
            return new GenreView
            {
                Name = model.Name,
                Id = model.Id
            };
        }
    }
}