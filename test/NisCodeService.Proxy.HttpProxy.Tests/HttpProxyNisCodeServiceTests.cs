namespace NisCodeService.Proxy.HttpProxy.Tests;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Moq;
using Moq.Protected;
using Xunit;

public class HttpProxyNisCodeServiceTests
{
    private static HttpClient MockHttpClient<T>(T? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        HttpContent? content = null;
        if (typeof(T) == typeof(Dictionary<string, string>))
        {
            content = JsonContent.Create(value);
        }

        if (typeof(T) == typeof(string))
        {
            content = new StringContent(value.ToString()!);
        }

        if (content is null)
        {
            throw new InvalidProgramException("Value parameter should be of type string or Dictionary<string, string>.");
        }

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpResponseMessage = new HttpResponseMessage { Content = content };

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(nameof(HttpClient.SendAsync), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // create the HttpClient
        var httpClient = new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        return httpClient;
    }

    [Fact]
    public async Task GetAll()
    {
        var expectedResult = new Dictionary<string, string>
        {
            ["002067"] = "44021"
        };
        var httpClient = MockHttpClient<Dictionary<string, string>>(expectedResult);

        var nisCodeService = new HttpProxyNisCodeService(httpClient: httpClient);
        var result = await nisCodeService.GetAll();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task Get()
    {
        var expectedResult = "44021";
        var httpClient = MockHttpClient<string>(expectedResult);

        var nisCodeService = new HttpProxyNisCodeService(httpClient: httpClient);
        var result = await nisCodeService.Get("OVO002067".WithoutOvoPrefix()!);

        Assert.Equal(expectedResult, result);
    }
}
