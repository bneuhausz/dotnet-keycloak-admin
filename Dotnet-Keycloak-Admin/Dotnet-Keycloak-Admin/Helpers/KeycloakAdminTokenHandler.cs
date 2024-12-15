using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Nbx.DotnetKeycloak.Admin.Configuration;

namespace Nbx.DotnetKeycloak.Admin.Helpers;

public class KeycloakAdminTokenHandler : DelegatingHandler
{
    private readonly KeycloakAdminOptions _keycloakConfig;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public KeycloakAdminTokenHandler(IOptions<KeycloakAdminOptions> keycloakConfig, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _keycloakConfig = keycloakConfig.Value;
        _httpContextAccessor = httpContextAccessor;

        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(_keycloakConfig.BaseAddress);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri != null && IsAdminRequest(request.RequestUri))
        {
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private bool IsAdminRequest(Uri requestUri)
    {
        var baseAddress = new Uri(_keycloakConfig.BaseAddress);
        return requestUri.AbsoluteUri.StartsWith(baseAddress.AbsoluteUri) && requestUri.AbsolutePath.StartsWith("/admin/");
    }

    private async Task<string> GetAccessToken()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            //TODO: proper error handling
            throw new InvalidOperationException("No HttpContext available.");
        }

        if (context.Items["KeycloakAccessToken"] is string cachedToken)
        {
            return cachedToken;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _keycloakConfig.ClientId,
            ["client_secret"] = _keycloakConfig.ClientSecret,
            ["grant_type"] = "client_credentials"
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        using JsonDocument document = JsonDocument.Parse(responseContent);
        JsonElement root = document.RootElement;
        string accessToken = root.GetProperty("access_token").GetString()!;
        context.Items["KeycloakAccessToken"] = accessToken;

        return accessToken;
    }
}
