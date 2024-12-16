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
    Task ResetPasswordAsync(string id, CredentialDto credential);
    Task<GetClientDto> GetClientAsync();
    Task<List<GetRoleDto>> GetUserClientRolesAsync(string id, string clientId);
    Task<List<GetRoleDto>> GetAvailableClientRolesAsync(string id, string clientId);
    Task AssignRoleToUserAsync(string id, AssignRoleDto role);
    Task RemoveRoleFromUserAsync(string id, AssignRoleDto role);
}
