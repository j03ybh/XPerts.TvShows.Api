using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using XPertz.TvShows.Database.Migrator.Logging;

namespace XPertz.TvShows.Database.Migrator
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", false, true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                    .RegisterCoreServices()
                    .RegisterOptions(hostContext);
                });

            builder.ConfigureLogging(logging =>
            {
                logging.AddConsoleLogger();
            });

            try
            {
                return await builder.RunCommandLineApplicationAsync<MainCommand>(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command failed with unexpected error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");
                }

                return -1;
            }
        }
    }
}