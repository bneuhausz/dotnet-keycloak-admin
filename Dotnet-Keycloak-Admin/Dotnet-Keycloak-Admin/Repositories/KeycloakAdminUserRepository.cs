using Nbx.DotnetKeycloak.Admin.Configuration;
using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Nbx.DotnetKeycloak.Admin.Entities.Keycloak;
using Nbx.DotnetKeycloak.Admin.Dtos.Role;
using Nbx.DotnetKeycloak.Admin.Dtos.Client;

namespace Nbx.DotnetKeycloak.Admin.Repositories;

public class KeycloakAdminUserRepository : IKeycloakAdminUserRepository
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _keycloakConfig;
    private static JsonSerializerOptions CamelCaseJsonSerializer => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public KeycloakAdminUserRepository(HttpClient httpClient, IOptions<KeycloakAdminOptions> keycloakConfig)
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

    public async Task<GetClientDto> GetClientAsync()
    {
        var clientReq = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/clients?clientId={_keycloakConfig.PublicClientId}", HttpMethod.Get);
        var clientRes = await _httpClient.SendAsync(clientReq);
        clientRes.EnsureSuccessStatusCode();

        var clientResContent = await clientRes.Content.ReadAsStringAsync();
        var clients = JsonSerializer.Deserialize<List<GetClientDto>>(clientResContent, CamelCaseJsonSerializer);
        var client = clients?.FirstOrDefault();
        if (client == null)
        {
            //TODO: proper error handling
            throw new Exception($"Client with ID '{_keycloakConfig.ClientId}' not found.");
        }
        else
        {
            return client;
        }
    }

    //TODO: refactor this to return both the roles of the user and the availabe roles
    public async Task<List<GetRoleDto>> GetUserClientRolesAsync(string id, string clientId)
    {
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users/{id}/role-mappings/clients/{clientId}", HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        var roles = JsonSerializer.Deserialize<List<GetRoleDto>>(resContent, CamelCaseJsonSerializer);
        return roles ?? [];
    }

    public async Task<List<GetRoleDto>> GetAvailableClientRolesAsync(string userId, string clientId)
    {
        var req = CreateRequest($"admin/realms/{_keycloakConfig.Realm}/users/{userId}/role-mappings/clients/{clientId}/available", HttpMethod.Get);
        var res = await _httpClient.SendAsync(req);
        res.EnsureSuccessStatusCode();
        var resContent = await res.Content.ReadAsStringAsync();
        var roles = JsonSerializer.Deserialize<List<GetRoleDto>>(resContent, CamelCaseJsonSerializer);
        return roles ?? [];
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
