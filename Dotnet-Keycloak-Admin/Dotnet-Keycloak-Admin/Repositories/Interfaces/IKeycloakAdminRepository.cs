using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Entities.Keycloak;

namespace Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;

public interface IKeycloakAdminRepository
{
    Task<int> GetUserCountAsync(string username);
    Task<List<GetUserDto>> GetUsersAsync(int first, int max, string username);
    Task<UserRepresentation?> GetUserByIdAsync(string id);
    Task CreateUserAsync(CreateUserDto user);
    Task ToggleUserEnabledAsync(string id);
    Task ResetPassword(string id, CredentialDto credential);
}
