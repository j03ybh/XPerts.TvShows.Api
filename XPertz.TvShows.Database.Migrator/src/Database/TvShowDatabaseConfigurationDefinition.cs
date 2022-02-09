using TechMinimalists.Database.Configuration;
using TechMinimalists.Database.Sql.Configuration;
using XPerts.TvShows.Database;

namespace XPertz.TvShows.Database.Migrator.Database
{
    public class TvShowDatabaseConfigurationDefinition : IDatabaseConfigurationDefinition
    {
        public TvShowDatabaseConfigurationDefinition()
        {
        }

        public ISchemaConfiguration[] GetSchemaConfigurations()
        {
            return new ISchemaConfiguration[]
            {
                new SqlSchemaConfiguration("TV")
            };
        }

        public IDatabaseTableConfiguration[] GetTableConfigurations()
        {
            return new IDatabaseTableConfiguration[]
            {
                new TvShowTableConfiguration(),
                new GenreTableConfiguration(),
                new TvShowGenreTableConfiguration()
            };
        }
    }
}