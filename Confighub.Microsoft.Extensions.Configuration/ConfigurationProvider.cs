using Confighub.Microsoft.Extensions.Configuration.Api;
using Confighub.Microsoft.Extensions.Configuration.Api.Models.Request;
using Confighub.Microsoft.Extensions.Configuration.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Confighub.Microsoft.Extensions.Configuration
{
    internal class ConfigurationProvider : JsonConfigurationProvider
    {
        private readonly IConfigurationRoot _configurationRoot;
        private readonly ILogger<ConfigurationProvider> _logger;
        internal ConfigurationProvider(JsonConfigurationSource source, IConfigurationRoot configurationRoot, ILoggerFactory loggerFactory) : base(source, loggerFactory)
        {
            _configurationRoot = configurationRoot;
            _logger = loggerFactory?.CreateLogger<ConfigurationProvider>();
        }
        public override void Load()
        {
            var configHubOptions = new Options();
            _configurationRoot.GetSection("ConfigHub").Bind(configHubOptions);

            if (configHubOptions.EnableSync)
            {
                var pushId = Guid.NewGuid();
                try
                {
                    _logger?.LogInformation("Push is enabled, starting synchronization.");
                    SetDepth(1);
                    base.Load();
                    _logger?.LogInformation($"Load from appsettings.json succeed. pulling remote configuration from {configHubOptions.Url}.");
                    var chClient = new Client(configHubOptions);
                    var pullTask = chClient.PullAsync();
                    Task.WaitAll(pullTask);
                    var remoteConfiguration = pullTask.Result;
                    _logger?.LogInformation($"Pull succeed. config info: {remoteConfiguration.Account} - {remoteConfiguration.Context} - {remoteConfiguration.Repository} - {remoteConfiguration.GeneratedOn}.");
                    var push = new Push
                    {
                        ChangeComment = "Syncronization - " + pushId,
                        EnableKeyCreation = configHubOptions.EnableKeyCreation,
                        Data = Data.Keys.Select(key =>
                        {
                            var extractedValue = Data.ExtractKeyValue(key);
                            return new PushData
                            {
                                Key = extractedValue.Item1,
                                Type = extractedValue.Item3,
                                Deprecated = false,
                                Push = true,
                                Values = new List<PushDataValue>
                                {
                                    new PushDataValue
                                    {
                                        Active = true,
                                        Context = string.Join(";", configHubOptions.PushContexts),
                                        Value = extractedValue.Item2
                                    }
                                }
                            };
                        })
                        .Union(remoteConfiguration.Properties.Where(kvp => !Data.Keys
                            .Any(key => key.StartsWith(kvp.Key.Replace(".", ":"))))
                            .Select(kvp =>
                            {
                                return new PushData
                                {
                                    Key = kvp.Key,
                                    Type = kvp.Value.Type,
                                    Push = true,
                                    //Operation = kvp.Value.Type == "List" ? "delete" : "",
                                    Values = new List<PushDataValue>{
                                            new PushDataValue {
                                                Context = string.Join(";", configHubOptions.PushContexts),
                                                Operation = "delete"
                                            }
                                        }
                                };
                            })).ToList()
                    };
                    _logger?.LogInformation($"Pushing changes {pushId}.");
                    var pushTask = chClient.PushAsync(push);
                    Task.WaitAll(pushTask);
                    _logger?.LogInformation($"Push {pushId} succeed.");
                }
                catch (Exception ex)
                {
                    _logger?.LogCritical($"Syncronization {pushId} failed", ex);
                    if (configHubOptions.ThrowExceptions)
                        throw ex;
                }
            }
            if (configHubOptions.UpdateAppSettings)
            {
                try
                {
                    _logger?.LogInformation($"Pulling remote configuration from {configHubOptions.Url}.");
                    var chClient = new Client(configHubOptions);
                    var pullTask = chClient.PullAsync();
                    Task.WaitAll(pullTask);
                    var remoteConfiguration = pullTask.Result;
                    _logger?.LogInformation($"Pull succeed. config info: {remoteConfiguration.Account} - {remoteConfiguration.Context} - {remoteConfiguration.Repository} - {remoteConfiguration.GeneratedOn}.");
                    Store(remoteConfiguration);
                    SetDepth(-1);
                }
                catch (Exception ex)
                {
                    _logger?.LogCritical($"Pulling failed.", ex);
                    if (configHubOptions.ThrowExceptions)
                        throw ex;
                }
                finally
                {
                    base.Load();
                }
            }
        }
    }
}
