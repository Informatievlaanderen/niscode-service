namespace NisCodeService.Proxy.HttpProxy;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;

public class HttpProxyNisCodeService : INisCodeService
{
    private readonly HttpClient _httpClient;

    public HttpProxyNisCodeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default) =>
        await _httpClient.GetFromJsonAsync<Dictionary<string, string>>("/niscode", cancellationToken) ?? new Dictionary<string, string>();

    public async Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default) =>
        await _httpClient.GetStringAsync($"/niscode/{ovoCode}", cancellationToken: cancellationToken);
}
