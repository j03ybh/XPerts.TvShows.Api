using TechMinimalists.Database.Core;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.Database
{
    public sealed class GenreMapper : ModelMapper<Genre>
    {
        public GenreMapper()
        {
        }

        protected override IDictionary<string, object> MapBackCore(Genre model)
        {
            return new Dictionary<string, object>(2)
            {
                [nameof(model.Id)] = model.Id,
                [nameof(model.Name)] = model.Name
            };
        }

        protected override Genre MapCore(IDictionary<string, object> values)
        {
            var result = new Genre();
            var id = values.GetInteger(nameof(result.Id));
            var name = values.GetString(nameof(result.Name));

            if (id.HasValue)
                result.Id = id.Value;
            result.Name = name;

            return result;
        }
    }
}