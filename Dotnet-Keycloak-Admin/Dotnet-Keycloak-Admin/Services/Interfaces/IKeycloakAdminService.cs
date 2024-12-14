namespace Dotnet_Keycloak_Admin.Services.Interfaces;

public interface IKeycloakAdminService
{
    Task<string> Test();
}
