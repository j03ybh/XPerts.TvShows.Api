using TechMinimalists.Database.Core;

namespace XPertz.TvShows.Controllers
{
    public interface IDataController<TModel, TModelView>
        where TModel : class
        where TModelView : class
    {
        Task<TModelView> AddAsync(TModelView modelView, CancellationToken cancellationToken = default);

        Task<TModelView> UpdateAsync(TModelView modelView, CancellationToken cancellationToken = default);

        Task AddAsync(IEnumerable<TModelView> modelViews, CancellationToken cancellationToken = default);

        Task<TModelView> GetAsync(object id, CancellationToken cancellationToken = default);

        Task<IPage<TModelView>> GetAsync(int pageNumber = 1, CancellationToken cancellationToken = default);

        Task<IEnumerable<TModelView>> QueryAsync(ColumnQuery[] columnQueries, CancellationToken cancellationToken = default);

        Task DeleteAsync(object id, CancellationToken cancellationToken = default);
    }
}