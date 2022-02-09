namespace XPertz.TvShows.Controllers
{
    public interface IPage<TModelView>
        where TModelView : class
    {
        IEnumerable<TModelView> Values { get; }

        int Count => Values?.Count() ?? 0;

        int PageNumber { get; }
    }
}