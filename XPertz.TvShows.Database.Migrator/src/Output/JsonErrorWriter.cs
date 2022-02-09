using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace XPertz.TvShows.Database.Migrator.Output
{
    public class JsonErrorWriter
    {
        private readonly IConsole _console;

        public JsonErrorWriter(IConsole console)
        {
            _console = console;
        }

        public void Output(string message)
        {
            var jsonMessage = JsonConvert.SerializeObject(new
            {
                Error = message
            }, Formatting.Indented);

            WriteJson(jsonMessage);
        }

        public void Output(object obj)
        {
            var jsonMessage = JsonConvert.SerializeObject(obj, Formatting.Indented);

            WriteJson(jsonMessage);
        }

        public void Output(Exception e)
        {
            Output(SerializeException(e));
        }

        private void WriteJson(string jsonMessage)
        {
            _console.Out.WriteLine(string.Empty);
            _console.BackgroundColor = ConsoleColor.Red;
            _console.ForegroundColor = ConsoleColor.White;
            _console.Out.Write(jsonMessage);
            _console.ResetColor();
        }

        private object SerializeException(Exception e)
        {
            return new
            {
                e.Message,
                e.StackTrace,
                InnerException = e.InnerException is not null
                    ? SerializeException(e.InnerException)
                    : null
            };
        }
    }
}