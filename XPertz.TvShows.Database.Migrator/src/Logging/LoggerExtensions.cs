using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace XPertz.TvShows.Database.Migrator.Logging
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, ConsoleLoggerProvider>());

            return builder;
        }
    }
}