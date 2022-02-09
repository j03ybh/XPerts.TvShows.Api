using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using XPertz.TvShows.Database.Migrator.Output;

namespace XPertz.TvShows.Database.Migrator.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly JsonOutputWriter _writer;
        private readonly JsonErrorWriter _errorWriter;
        private readonly string _categoryName;

        public ConsoleLogger(string categoryName, JsonOutputWriter writer, JsonErrorWriter errorWriter)
        {
            _categoryName = categoryName;
            _writer = writer;
            _errorWriter = errorWriter;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            var logMessage = string.Format(
                "[{0}] Category '{1}': \"{2}\"",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                _categoryName,
                message
            );

            if (exception is null)
                _writer.Output(logMessage);
            else
                _errorWriter.Output(exception);
        }
    }

    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private JsonOutputWriter _writer;
        private JsonErrorWriter _errorWriter;
        private bool disposed;

        public ConsoleLoggerProvider(IConsole console)
        {
            _writer = new JsonOutputWriter(console);
            _errorWriter = new JsonErrorWriter(console);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger(categoryName, _writer, _errorWriter);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _writer = null;
                    _errorWriter = null;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}