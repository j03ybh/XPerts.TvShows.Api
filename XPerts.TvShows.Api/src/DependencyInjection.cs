using TechMinimalists.Database.Core;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Repositories;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Interfaces;
using TechMinimalists.Mapping;
using XPerts.TvShows.Database;
using XPerts.TvShows.Mappers;
using XPerts.TvShows.Models;
using XPerts.TvShows.StatementConstructors;
using XPertz.TvShows.Controllers;
using XPertz.TvShows.Repositories;

namespace XPerts.TvShows.Api.Extensions
{
    internal static class DependencyInjection
    {
        internal static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ISqlConnectionProvider, SqlConnectionProvider>()
                .AddScoped<ISqlStatementExecutor, SqlStatementExecutor>()

                .AddScoped<IStatementConstructor<TvShow>, TvShowStatementConstructor>()
                .AddScoped<IStatementConstructor<Genre>, GenreStatementConstructor>()
                .AddScoped<IStatementConstructor<TvShowGenre>, TvShowGenreStatementConstructor>()

                .AddScoped<IModelMapper<TvShow>, TvShowMapper>()
                .AddScoped<IModelMapper<Genre>, GenreMapper>()
                .AddScoped<IModelMapper<TvShowGenre>, TvShowGenreMapper>()
                .AddScoped<IModelViewMapper<TvShow, TvShowView>, TvShowViewMapper>()
                .AddScoped<IModelViewMapper<Genre, GenreView>, GenreViewMapper>()

                .AddScoped<IReadWriteRepository<Genre>, GenreRepository>()
                .AddScoped<IReadWriteRepository<TvShow>, TvShowRepository>()
                .AddScoped<IJoinedRepository<TvShowGenre>, TvShowGenreRepository>()

                .AddScoped<IPageCollection<TvShowView>, PageCollection<TvShowView>>()

                .AddScoped<IDataController<TvShow, TvShowView>, TvShowDataController>();
        }

        internal static IServiceCollection RegisterOptions(this IServiceCollection services, HostBuilderContext hostContext)
        {
            return services
                .Configure<DatabaseOptions>(hostContext.Configuration.GetSection(DatabaseOptions.OptionsName))
                .Configure<PaginationOptions>(hostContext.Configuration.GetSection(PaginationOptions.OptionsName));
        }
    }
}