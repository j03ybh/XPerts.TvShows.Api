using McMaster.Extensions.CommandLineUtils;
using XPertz.TvShows.Database.Migrator.Database;

namespace XPertz.TvShows.Database.Migrator
{
    [Command(Name = "create-database")]
    internal class CreateDatabaseCommand : CommandBase
    {
        private readonly IDatabaseCreator _databaseCreator;

        public CreateDatabaseCommand(IConsole console, IDatabaseCreator databaseCreator) : base(console)
        {
            _databaseCreator = databaseCreator;
        }

        protected override Task<int> OnInnerExecuteAsync(CommandLineApplication app)
        {
            _databaseCreator.CreateDatabase();

            return Task.FromResult(0);
        }
    }
}