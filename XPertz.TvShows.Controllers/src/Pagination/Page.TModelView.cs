using System.Text.Json.Serialization;

namespace XPertz.TvShows.Controllers
{
    public sealed class Page<TModelView> : IPage<TModelView>
        where TModelView : class
    {
        [JsonConstructor]
        public Page(TModelView[] values, int pageNumber)
        {
            Values = values;
            PageNumber = pageNumber;
        }

        public IEnumerable<TModelView> Values { get; }

        public int PageNumber { get; }
    }
}