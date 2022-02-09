using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace XPertz.TvShows.Database.Migrator.Output
{
    public class JsonOutputWriter
    {
        private readonly IConsole _console;

        public JsonOutputWriter(IConsole console)
        {
            _console = console;
        }

        public void Output(string message)
        {
            WriteText(message);
        }

        public void Output(object obj)
        {
            var jsonMessage = JsonConvert.SerializeObject(obj, Formatting.Indented);

            WriteText(jsonMessage);
        }

        private void WriteText(string text)
        {
            _console.Out.WriteLine(string.Empty);
            _console.BackgroundColor = ConsoleColor.Black;
            _console.ForegroundColor = ConsoleColor.White;
            _console.Out.Write(text);
            _console.ResetColor();
        }
    }
}