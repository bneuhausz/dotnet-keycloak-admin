using Dotnet_Keycloak_Admin.Dtos.User;
using Dotnet_Keycloak_Admin.Repositories.Interfaces;
using Dotnet_Keycloak_Admin.Services.Interfaces;

namespace Dotnet_Keycloak_Admin.Services;

public class KeycloakAdminService : IKeycloakAdminService
{
    private readonly IKeycloakAdminRepository _keycloakAdminRepository;

    public KeycloakAdminService(IKeycloakAdminRepository keycloakAdminRepository)
    {
        _keycloakAdminRepository = keycloakAdminRepository;
    }

    public async Task<List<GetUserDto>> GetUsersAsync()
    {
        return await _keycloakAdminRepository.GetUsersAsync();
    }
}
