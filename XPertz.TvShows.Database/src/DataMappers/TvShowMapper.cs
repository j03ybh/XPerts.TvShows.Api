using System.Text;
using TechMinimalists.Database.Core;
using XPerts.TvShows.Models;

namespace XPerts.TvShows.Database
{
    public sealed class TvShowMapper : ModelMapper<TvShow>
    {
        public TvShowMapper()
        {
        }

        protected override IDictionary<string, object> MapBackCore(TvShow model)
        {
            var result = new Dictionary<string, object>(4)
            {
                [nameof(model.Id)] = model.Id,
                [nameof(model.Name)] = model.Name,
                [nameof(model.PremieredOn)] = model.PremieredOn
            };

            if (model.OriginId.HasValue && model.OriginId.Value > 0)
                result[nameof(model.OriginId)] = model.OriginId.Value;

            return result;
        }

        protected override TvShow MapCore(IDictionary<string, object> values)
        {
            var result = new TvShow();

            var id = values.GetInteger(nameof(result.Id));
            if (id.HasValue)
                result.Id = id.Value;
            var indexPosition = values.GetLong(nameof(result.IndexPosition));
            if (indexPosition.HasValue)
                result.IndexPosition = indexPosition.Value;
            var originId = values.GetLong(nameof(result.OriginId));
            if (originId.HasValue)
                result.OriginId = originId.Value;

            result.PremieredOn = values.GetDateTime(nameof(result.PremieredOn));
            result.Name = values.GetString(nameof(result.Name));

            return result;
        }
    }
}