namespace XPertz.TvShows.Controllers
{
    public interface IPageCollection<TModelView>
        where TModelView : class
    {
        Task<IPage<TModelView>> GetOrCachePageAsync(int pageNumber, Func<PageIndexColumnQuery, Task<IEnumerable<TModelView>>> pageFactory);

        void RefreshPage(long modelIndexPosition);
    }
}