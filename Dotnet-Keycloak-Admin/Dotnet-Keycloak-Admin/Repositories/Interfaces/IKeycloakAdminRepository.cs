using Dotnet_Keycloak_Admin.Dtos.User;

namespace Dotnet_Keycloak_Admin.Repositories.Interfaces;

public interface IKeycloakAdminRepository
{
    Task<List<GetUserDto>> GetUsersAsync();
}
