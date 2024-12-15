using System.Text.Json.Serialization;

namespace Nbx.DotnetKeycloak.Admin.Entities.Keycloak;

public record UserRepresentation
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }

    [JsonPropertyName("requiredActions")]
    public List<string>? RequiredActions { get; set; }

    [JsonPropertyName("groups")]
    public List<string>? Groups { get; set; }
}