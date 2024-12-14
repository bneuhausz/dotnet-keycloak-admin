namespace Dotnet_Keycloak_Admin.Configuration;

public class KeycloakAdminOptions
{
    public const string Section = "KeycloakAdmin";
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string BaseAddress { get; set; }
}
