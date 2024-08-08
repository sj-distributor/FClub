using Serilog;
using Autofac;
using Newtonsoft.Json;
using FClub.Core.Ioc;

namespace FClub.Core.Services.Http;

public interface IFClubHttpClientFactory : IScopedDependency
{
    Task<T> GetAsync<T>(string requestUrl, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, HttpClient innerClient = null, bool shouldThrow = false);
    
    Task<HttpResponseMessage> GetAsync(string requestUrl, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, HttpClient innerClient = null, bool shouldThrow = false);
    
    Task<T> PostAsync<T>(string requestUrl, HttpContent content, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false);

    Task<HttpResponseMessage> PostAsync(string requestUrl, HttpContent content, CancellationToken cancellationToken,
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false);
    
    Task<T> PostAsJsonAsync<T>(string requestUrl, object value, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false);
    
    Task<HttpResponseMessage> PostAsJsonAsync(string requestUrl, object value, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false);
    
    HttpClient CreateClient(TimeSpan? timeout = null, bool beginScope = false,
        Dictionary<string, string> headers = null, HttpClient innerClient = null);
}

public class FClubHttpClientFactory : IFClubHttpClientFactory
{
    private readonly ILifetimeScope _scope;

    public FClubHttpClientFactory(ILifetimeScope scope)
    {
        _scope = scope;
    }

    public HttpClient CreateClient(TimeSpan? timeout = null, bool beginScope = false,
        Dictionary<string, string> headers = null, HttpClient innerClient = null)
    {
        if (innerClient != null) return innerClient;

        var scope = beginScope ? _scope.BeginLifetimeScope() : _scope;

        var canResolve = scope.TryResolve(out IHttpClientFactory httpClientFactory);

        var client = canResolve ? httpClientFactory.CreateClient() : new HttpClient();

        if (timeout != null)
            client.Timeout = timeout.Value;

        if (headers == null) return client;

        foreach (var header in headers)
        {
            client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        return client;
    }
    
    public async Task<T> GetAsync<T>(string requestUrl, CancellationToken cancellationToken,
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, HttpClient innerClient = null, bool shouldThrow = false)
    {
        return await SafelyProcessRequestAsync(requestUrl, async () =>
        {
            var response = await CreateClient(timeout: timeout, beginScope: beginScope, headers: headers, innerClient)
                .GetAsync(requestUrl, cancellationToken).ConfigureAwait(false);
            
            return await ReadAndLogResponseAsync<T>(requestUrl, HttpMethod.Get, response, cancellationToken).ConfigureAwait(false);
            
        }, cancellationToken, shouldThrow).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUrl, CancellationToken cancellationToken,
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, HttpClient innerClient = null, bool shouldThrow = false)
    {
        return await SafelyProcessRequestAsync(requestUrl, async () =>
                await CreateClient(timeout: timeout, beginScope: beginScope, headers: headers, innerClient)
                    .GetAsync(requestUrl, cancellationToken).ConfigureAwait(false), cancellationToken, shouldThrow).ConfigureAwait(false);
    }

    public async Task<T> PostAsync<T>(string requestUrl, HttpContent content, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false)
    {
        return await SafelyProcessRequestAsync(requestUrl, async () =>
        {
            var response = await CreateClient(timeout: timeout, beginScope: beginScope, headers: headers)
                .PostAsync(requestUrl, content, cancellationToken).ConfigureAwait(false);

            return await ReadAndLogResponseAsync<T>(requestUrl, HttpMethod.Post, response, cancellationToken).ConfigureAwait(false);
            
        }, cancellationToken, shouldThrow).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUrl, HttpContent content, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false)
    {
        return await SafelyProcessRequestAsync(requestUrl, async () =>
                await CreateClient(timeout: timeout, beginScope: beginScope, headers: headers)
                    .PostAsync(requestUrl, content, cancellationToken).ConfigureAwait(false), cancellationToken, shouldThrow).ConfigureAwait(false);
    }
    
    public async Task<T> PostAsJsonAsync<T>(string requestUrl, object value, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false)
    {
        return await SafelyProcessRequestAsync(requestUrl, async () =>
        {
            var response = await CreateClient(timeout: timeout, beginScope: beginScope, headers: headers)
                .PostAsJsonAsync(requestUrl, value, cancellationToken).ConfigureAwait(false);

            return await ReadAndLogResponseAsync<T>(requestUrl, HttpMethod.Post, response, cancellationToken).ConfigureAwait(false);
            
        }, cancellationToken, shouldThrow).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUrl, object value, CancellationToken cancellationToken, 
        TimeSpan? timeout = null, bool beginScope = false, Dictionary<string, string> headers = null, bool shouldThrow = false)
    {
        return await SafelyProcessRequestAsync(requestUrl, async () =>
                await CreateClient(timeout: timeout, beginScope: beginScope, headers: headers)
                    .PostAsJsonAsync(requestUrl, value, cancellationToken).ConfigureAwait(false), cancellationToken, shouldThrow).ConfigureAwait(false);
    }

    private static async Task<T> ReadAndLogResponseAsync<T>(string requestUrl, HttpMethod httpMethod, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return await ReadResponseContentAs<T>(response, cancellationToken).ConfigureAwait(false);
        
        LogHttpError(requestUrl, httpMethod, response);

        return default;
    }

    private static async Task<T> ReadResponseContentAs<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (typeof(T) == typeof(string))
            return (T)(object) await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (typeof(T) == typeof(byte[]))
            return (T)(object) await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    private static void LogHttpError(string requestUrl, HttpMethod httpMethod, HttpResponseMessage response)
    {
        Log.Error("PostBoy http {Method} {Url} error, The response: {ResponseJson}", 
            httpMethod.ToString(), requestUrl, JsonConvert.SerializeObject(response));
    }
    
    private static async Task<T> SafelyProcessRequestAsync<T>(string requestUrl, Func<Task<T>> func, CancellationToken cancellationToken, bool shouldThrow = false)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return await func().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error on requesting {RequestUrl}", requestUrl);
            
            if (shouldThrow)
                throw;
            return default;
        }
    }
}