namespace Dotnet_Keycloak_Admin.Repositories.Interfaces;

public interface IKeycloakAdminRepository
{
    Task<string> Test();
}
