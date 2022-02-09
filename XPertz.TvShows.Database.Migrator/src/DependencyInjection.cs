using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechMinimalists.Clients.Core;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Core.Interfaces;
using TechMinimalists.Database.Sql;
using TechMinimalists.Database.Sql.Interfaces;
using XPerts.TvShows.Database;
using XPerts.TvShows.Models;
using XPertz.TvShows.Database.Migrator.Database;
using XPertz.TvShows.Database.Migrator.Sync;

namespace XPertz.TvShows.Database.Migrator
{
    internal static class DependencyInjection
    {
        internal static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IApiClient, ApiClient>()
                .AddScoped<IMazeTvApiClient, MazeTvApiClient>()

                .AddScoped<ITvShowSyncManager, TvShowSyncManager>()
                .AddScoped<ITvShowSyncFilter, TvShowSyncFilter>()
                .AddScoped<ITvShowSyncReader, TvShowSyncReader>()
                .AddScoped<ITvShowSyncWriter, TvShowSyncWriter>()
                .AddScoped<IStatementExecutor, SqlStatementExecutor>()
                .AddScoped<ISqlConnectionProvider, DatabaseSyncConnectionProvider>()

                .AddScoped<IModelMapper<TvShow>, TvShowMapper>()
                .AddScoped<IModelMapper<Genre>, GenreMapper>()
                .AddScoped<IModelMapper<TvShowGenre>, TvShowGenreMapper>()

                .AddScoped<IDatabaseCreator, DatabaseCreator>()
                .AddScoped<IDatabaseConfigurationDefinition, TvShowDatabaseConfigurationDefinition>()
                .AddScoped<IDatabaseConfigurationDefinition, TvShowDatabaseConfigurationDefinition>();
        }

        internal static IServiceCollection RegisterOptions(this IServiceCollection services, HostBuilderContext hostContext)
        {
            return services
                .Configure<DatabaseOptions>(hostContext.Configuration.GetSection(DatabaseOptions.OptionsName))
                .Configure<TvShowSyncFilterOptions>(hostContext.Configuration.GetSection(TvShowSyncFilterOptions.OptionsName))
                .Configure<ApiOptions>(hostContext.Configuration.GetSection(ApiOptions.OptionsName));
        }
    }
}