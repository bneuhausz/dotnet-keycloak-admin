using Dotnet_Keycloak_Admin.Configuration;
using Dotnet_Keycloak_Admin.Dtos.User;
using Dotnet_Keycloak_Admin.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Dotnet_Keycloak_Admin.Repositories;

public class KeycloakAdminRepository : IKeycloakAdminRepository
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _keycloakConfig;
    private static JsonSerializerOptions CamelCaseJsonSerializer => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public KeycloakAdminRepository(HttpClient httpClient, IOptions<KeycloakAdminOptions> keycloakConfig)
    {
        _httpClient = httpClient;
        _keycloakConfig = keycloakConfig.Value;
    }

    public async Task<int> GetUserCountAsync()
    {
        var req = await CreateRequest($"/admin/realms/{_keycloakConfig.Realm}/users/count", HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        var cnt = JsonSerializer.Deserialize<int>(resContent);
        return cnt;
    }

    public async Task<List<GetUserDto>> GetUsersAsync()
    {
        var req = await CreateRequest($"/admin/realms/{_keycloakConfig.Realm}/users", HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<GetUserDto>>(resContent, CamelCaseJsonSerializer);
        return users ?? [];
    }

    private async Task<HttpRequestMessage> CreateRequest(string endpoint, HttpMethod httpMethod)
    {
        var accessToken = await GetAccessToken();
        var uri = $"{_httpClient.BaseAddress}{endpoint}";
        var req = new HttpRequestMessage()
        {
            RequestUri = new Uri(uri),
            Method = httpMethod
        };

        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return req;
    }

    private async Task<string> GetAccessToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}/realms/{_keycloakConfig.Realm}/protocol/openid-connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _keycloakConfig.ClientId,
            ["client_secret"] = _keycloakConfig.ClientSecret,
            ["grant_type"] = "client_credentials"
        });

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        using JsonDocument document = JsonDocument.Parse(responseContent);
        JsonElement root = document.RootElement;
        string accessToken = root.GetProperty("access_token").GetString()!;
        return accessToken;
    }
}
