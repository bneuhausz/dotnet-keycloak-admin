using Nbx.DotnetKeycloak.Admin.Dtos.Client;
using Nbx.DotnetKeycloak.Admin.Dtos.Role;
using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Entities.Keycloak;

namespace Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;

public interface IKeycloakAdminUserRepository
{
    Task<int> GetUserCountAsync(string username);
    Task<List<GetUserDto>> GetUsersAsync(int first, int max, string username);
    Task<UserRepresentation?> GetUserByIdAsync(string id);
    Task CreateUserAsync(CreateUserDto user);
    Task ToggleUserEnabledAsync(string id);
    Task ResetPassword(string id, CredentialDto credential);
    Task<List<GetRoleDto>> GetUserRolesAsync(string id);
    Task<GetClientDto> GetClientByClientIdAsync(string clientId);
}
