using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using OctopusDeploy.Services;

namespace OctopusDeploy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();
            var releaseToBeRetained = config.GetValue<int>("AppSettings:ReleasesToBeRetained");
            var pathToInputFiles = config.GetValue<string>("AppSettings:PathToInputFiles");

            var servicesProvider = BuildDi(config);

            servicesProvider.GetRequiredService<IReleaseRetention>().RetainReleases(releaseToBeRetained, pathToInputFiles);
        }


        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
               .AddTransient<IReleaseRetention, ReleaseRetention>()
               .AddSingleton<IJsonReader<List<Models.Environment>>, JsonReader<List<Models.Environment>>>()
               .AddSingleton<IJsonReader<List<Models.Release>>, JsonReader<List<Models.Release>>>()
               .AddSingleton<IJsonReader<List<Models.Deployment>>, JsonReader<List<Models.Deployment>>>()
               .AddSingleton<IJsonReader<List<Models.Project>>, JsonReader<List<Models.Project>>>()
               .AddSingleton<IFileReader,FileReader>()
               .AddLogging(loggingBuilder =>
               {
                   // configure Logging with NLog
                   loggingBuilder.ClearProviders();
                   loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   loggingBuilder.AddNLog("nlog.config");

               })
               .BuildServiceProvider();
        }
    }
}
