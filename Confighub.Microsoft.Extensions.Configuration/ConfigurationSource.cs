using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace Confighub.Microsoft.Extensions.Configuration
{
    internal class ConfigurationSource : JsonConfigurationSource
    {
        private readonly IConfigurationRoot _configurationRoot;
        private readonly JsonConfigurationSource _appSettingsSource;
        private readonly ILoggerFactory _loggerFactory;

        public ConfigurationSource(IConfigurationRoot configurationRoot, JsonConfigurationSource appSettingsSource, ILoggerFactory loggerFactory)
        {
            _configurationRoot = configurationRoot;
            _appSettingsSource = appSettingsSource;
            _loggerFactory = loggerFactory;
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = _appSettingsSource.FileProvider;
            Optional = _appSettingsSource.Optional;
            Path = _appSettingsSource.Path;
            ReloadDelay = _appSettingsSource.ReloadDelay;
            ReloadOnChange = _appSettingsSource.ReloadOnChange;
            EnsureDefaults(builder);
            return new ConfigurationProvider(this, _configurationRoot, _loggerFactory);
        }
    }
}
