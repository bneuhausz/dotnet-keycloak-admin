using Nbx.DotnetKeycloak.Admin.Dtos.Role;
using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Requests;
using Nbx.DotnetKeycloak.Admin.Responses;

namespace Nbx.DotnetKeycloak.Admin.Services.Interfaces;

public interface IKeycloakAdminService
{
    Task<GetUsersResponse> GetUsersAsync(GetUsersRequest req);
    Task CreateUserAsync(CreateUserRequest req);
    Task ToggleUserEnabledAsync(string id);
    Task ResetPasswordAsync(string id, CredentialDto credential);
    Task<GetUserRolesResponse> GetUserRoleMappingsAsync(string id);
    Task AssignRoleToUserAsync(string id, AssignRoleRequest req);
    Task RemoveRoleFromUserAsync(string id, AssignRoleRequest req);
}
