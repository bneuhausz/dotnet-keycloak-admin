using Dotnet_Keycloak_Admin.Configuration;
using Dotnet_Keycloak_Admin.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dotnet_Keycloak_Admin.Repositories;

public class KeycloakAdminRepository : IKeycloakAdminRepository
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminOptions _keycloakConfig;

    public KeycloakAdminRepository(HttpClient httpClient, IOptions<KeycloakAdminOptions> keycloakConfig)
    {
        _httpClient = httpClient;
        _keycloakConfig = keycloakConfig.Value;
    }

    public async Task<string> Test()
    {
        var accessToken = await GetAccessToken();
        return accessToken;
    }

    private async Task<string> GetAccessToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}/protocol/openid-connect/token");
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
