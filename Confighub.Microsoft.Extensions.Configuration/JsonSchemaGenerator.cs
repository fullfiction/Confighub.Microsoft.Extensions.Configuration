using Confighub.Microsoft.Extensions.Configuration.Api.Models.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Confighub.Microsoft.Extensions.Configuration
{
    internal class JsonSchemaGenerator
    {
        private JsonSchemaGenerator() { }

        internal static IDictionary<string, object> Generate(Pull pull, ILoggerFactory loggerFactory) => new JsonSchemaGenerator().GenerateFromPull(pull, loggerFactory);

        private IDictionary<string, object> GenerateFromPull(Pull pull, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory?.CreateLogger<JsonSchemaGenerator>();
            var root = new Dictionary<string, object>();
            foreach (var key in pull.Properties.Keys)
            {
                var keys = key.Split('.');
                CreateJSONFromDictionary(root, keys, 0, pull.Properties[key], logger);
            }
            return root;
        }

        private static void CreateJSONFromDictionary(IDictionary<string, object> dict, string[] keys, int index, PullProperty value, ILogger<JsonSchemaGenerator> logger)
        {
            var key = keys[index];

            if (keys.Length > index + 1)
            {
                object childObj;
                IDictionary<string, object> nestedDict;
                if (dict.TryGetValue(key, out childObj))
                {
                    nestedDict = (IDictionary<string, object>)childObj;
                }
                else
                {
                    nestedDict = new Dictionary<string, object>();
                    dict[key] = nestedDict;
                }

                CreateJSONFromDictionary(nestedDict, keys, index + 1, value, logger);

            }
            else
            {
                try
                {
                    if (value.Type == "List")
                    {
                        var jsonString = (string)value.Value.ToString();
                        var array = JArray.Parse(jsonString);
                        var newArray = new JArray();
                        foreach (var item in array)
                        {
                            var itemValue = item.ToString();
                            if (itemValue.Contains("{"))
                                newArray.Add(JToken.Parse(item.ToString()));
                            else
                                newArray.Add(item);

                        }
                        dict[key] = newArray;
                    }
                    else
                        dict[key] = value.Value;
                }
                catch (Exception ex)
                {
                    logger?.LogCritical($"Filed to parse value for {key}, skipping", ex);
                    return;
                }
            }
        }
    }
}
