using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace Confighub.Microsoft.Extensions.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Configuration provider to work with confighub. Must be last provider in sequence. It uses appsettings.json file as source for local and remote synchronization.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loggerFactory">Optional, enables logging if provided</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddConfigHub(this IConfigurationBuilder builder, ILoggerFactory loggerFactory = null)
        {
            FlurlHttp.Configure(config =>
                config.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));

            var appSettingsSource = builder.Sources.Where(source =>
                (source is JsonConfigurationSource) &&
                (source as JsonConfigurationSource).Path == "appsettings.json")
                .Cast<JsonConfigurationSource>()
                .FirstOrDefault();
            appSettingsSource.Build(builder);
            var configRoot = builder.Build();
            return builder.Add(new ConfigurationSource(configRoot, appSettingsSource, loggerFactory));
        }
    }
}
