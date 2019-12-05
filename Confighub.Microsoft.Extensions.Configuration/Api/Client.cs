using Confighub.Microsoft.Extensions.Configuration.Api.Models.Request;
using Confighub.Microsoft.Extensions.Configuration.Api.Models.Response;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Confighub.Microsoft.Extensions.Configuration.Api
{
    public class Client
    {
        private readonly Options _options;

        public Client(Options options)
        {
            _options = options;
            _options.Validate();
            FlurlHttp.Configure(config => config.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            }));
        }

        public async Task<Pull> PullAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _options.Url
                    .AppendPathSegment(_options.PullPath)
                    .WithHeader("Client-Token", _options.Token)
                    .WithHeader("Context", string.Join(";", _options.PullContexts))
                    .WithHeader("Application-Name", _options.ApplicationName ?? "Uknown")
                    .GetJsonAsync<Pull>();
            return result;
        }

        public async Task PushAsync(Push push, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _options.Url
                        .AppendPathSegment(_options.PushPath)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithHeader("Client-Token", _options.Token)
                        .WithHeader("Application-Name", _options.ApplicationName)
                        .PostJsonAsync(push);
        }
    }
}
