using McMaster.Extensions.CommandLineUtils;
using XPertz.TvShows.Database.Migrator.Output;

namespace XPertz.TvShows.Database.Migrator
{
    [HelpOption("--help", ShortName = "h")]
    internal abstract class CommandBase
    {
        protected readonly IConsole Console;

        protected CommandBase(IConsole console)
        {
            Console = console;
        }

        protected abstract Task<int> OnInnerExecuteAsync(CommandLineApplication app);

        protected async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            try
            {
                return await OnInnerExecuteAsync(app);
            }
            catch (Exception e)
            {
                WriteError(e);
                return -1;
            }
        }

        protected void WriteOutput(object obj)
        {
            var outputer = new JsonOutputWriter(Console);
            outputer.Output(obj);
        }

        protected void WriteOutput(string message)
        {
            var outputer = new JsonOutputWriter(Console);
            outputer.Output(message);
        }

        protected void WriteError(object obj)
        {
            var outputer = new JsonErrorWriter(Console);
            outputer.Output(obj);
        }

        protected void WriteError(Exception e)
        {
            var outputer = new JsonErrorWriter(Console);
            outputer.Output(e);
        }
    }
}