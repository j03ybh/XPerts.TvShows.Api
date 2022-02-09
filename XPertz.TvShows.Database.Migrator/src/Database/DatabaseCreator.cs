using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMinimalists.Database.Core;
using TechMinimalists.Database.Sql;

namespace XPertz.TvShows.Database.Migrator.Database
{
    public class DatabaseCreator : IDatabaseCreator
    {
        private readonly IOptions<DatabaseOptions> _databaseOptions;
        private readonly IDatabaseConfigurationDefinition _databaseConfigurationDefinition;
        private readonly ILogger _logger;

        public DatabaseCreator(
            IOptions<DatabaseOptions> configuration,
            ILogger<DatabaseCreator> logger,
            IDatabaseConfigurationDefinition databaseConfigurationDefinition)
        {
            _databaseOptions = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            _databaseConfigurationDefinition = databaseConfigurationDefinition;
        }

        public void CreateDatabase()
        {
            _logger.LogInformation("Starting to create database {name}", _databaseOptions.Value?.Name);

            try
            {
                var database = new SqlDatabase(_databaseOptions, _databaseConfigurationDefinition.GetTableConfigurations());

                foreach (var schemaConfiguration in _databaseConfigurationDefinition.GetSchemaConfigurations())
                    database.AddSchemaConfiguration(schemaConfiguration);

                database.CreateDatabase();

                _logger.LogInformation("Successfully created database");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred when creating database");
            }
        }
    }
}