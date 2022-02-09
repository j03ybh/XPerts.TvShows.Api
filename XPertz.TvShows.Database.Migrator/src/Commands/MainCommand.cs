using McMaster.Extensions.CommandLineUtils;

namespace XPertz.TvShows.Database.Migrator
{
    [Subcommand(typeof(CreateDatabaseCommand))]
    [Subcommand(typeof(SyncDataCommand))]
    internal class MainCommand : CommandBase
    {
        public MainCommand(IConsole console)
            : base(console)
        { }

        protected override Task<int> OnInnerExecuteAsync(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }
}