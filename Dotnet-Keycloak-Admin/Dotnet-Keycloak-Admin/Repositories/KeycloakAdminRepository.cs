using Nbx.DotnetKeycloak.Admin.Configuration;
using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using Nbx.DotnetKeycloak.Admin.Entities.Keycloak;

namespace Nbx.DotnetKeycloak.Admin.Repositories;

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

    public async Task<int> GetUserCountAsync(string username)
    {
        var url = $"admin/realms/{_keycloakConfig.Realm}/users/count?q=type:regular&username={username}";
        var req = CreateRequest(url, HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        var cnt = JsonSerializer.Deserialize<int>(resContent);
        return cnt;
    }

    public async Task<List<GetUserDto>> GetUsersAsync(int first, int max, string username)
    {
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users?q=type:regular&first={first}&max={max}&username={username}", HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<GetUserDto>>(resContent, CamelCaseJsonSerializer);
        return users ?? [];
    }

    public async Task<UserRepresentation?> GetUserByIdAsync(string id)
    {
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users/{id}", HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
        var resContent = await res.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserRepresentation>(resContent, CamelCaseJsonSerializer);
        return user;
    }

    public async Task CreateUserAsync(CreateUserDto user)
    {
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users", HttpMethod.Post, user);
        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    public async Task ToggleUserEnabledAsync(string id)
    {
        var user = await GetUserByIdAsync(id);
        if (user == null)
        {
            //TODO: proper error handling
            throw new Exception("User not found");
        }
        user.Enabled = !user.Enabled;
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users/{id}", HttpMethod.Put, user);
        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    public async Task ResetPassword(string id, CredentialDto credential)
    {
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users/{id}/reset-password", HttpMethod.Put, credential);
        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage CreateRequest(string endpoint, HttpMethod httpMethod, object? content = null)
    {
        var uri = $"{_httpClient.BaseAddress}{endpoint}";
        var req = new HttpRequestMessage()
        {
            RequestUri = new Uri(uri),
            Method = httpMethod
        };

        if (content != null)
        {
            req.Content = JsonContent.Create(content, options: CamelCaseJsonSerializer);
        }

        return req;
    }
}
