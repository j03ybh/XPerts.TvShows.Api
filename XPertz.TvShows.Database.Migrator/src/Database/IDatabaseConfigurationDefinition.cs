using TechMinimalists.Database.Configuration;

namespace XPertz.TvShows.Database.Migrator.Database
{
    public interface IDatabaseConfigurationDefinition
    {
        IDatabaseTableConfiguration[] GetTableConfigurations();

        ISchemaConfiguration[] GetSchemaConfigurations();
    }
}