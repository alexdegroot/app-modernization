using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MutationProcessor.Database;
using MutationProcessor.Queue;

namespace MutationProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<Configuration>(hostContext.Configuration);
                    services.AddSingleton<IQueueReader, QueueReader>();
                    services.AddSingleton<IDatabaseWriter, DatabaseWriter>();
                    services.AddHostedService<Worker>();
                }) 
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                });
    }
}