using Dotnet_Keycloak_Admin.Dtos.User;

namespace Dotnet_Keycloak_Admin.Services.Interfaces;

public interface IKeycloakAdminService
{
    Task<List<GetUserDto>> GetUsersAsync();
}
