using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Sql.Interfaces;

namespace XPertz.TvShows.Database.Migrator.Sync
{
    public sealed class DatabaseSyncConnectionProvider : ISqlConnectionProvider
    {
        private readonly string _databaseName;

        public DatabaseSyncConnectionProvider(IOptions<DatabaseOptions> options)
        {
            if (options is null || options.Value is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.Value.Name))
                throw new ArgumentException("The database name in the Database options is not specified (appsettings.json)");

            _databaseName = options.Value.Name;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection($"Data Source=.;Initial Catalog={_databaseName};Integrated Security=True");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}