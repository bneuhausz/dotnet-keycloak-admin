using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;
using Nbx.DotnetKeycloak.Admin.Requests;
using Nbx.DotnetKeycloak.Admin.Responses;
using Nbx.DotnetKeycloak.Admin.Services.Interfaces;

namespace Nbx.DotnetKeycloak.Admin.Services;

public class KeycloakAdminService : IKeycloakAdminService
{
    private readonly IKeycloakAdminUserRepository _keycloakAdminRepository;

    public KeycloakAdminService(IKeycloakAdminUserRepository keycloakAdminRepository)
    {
        _keycloakAdminRepository = keycloakAdminRepository;
    }

    public async Task<GetUsersResponse> GetUsersAsync(GetUsersRequest req)
    {
        var getUserCountTask = _keycloakAdminRepository.GetUserCountAsync(req.Username);
        var getUsersTask = _keycloakAdminRepository.GetUsersAsync(req.First, req.Max, req.Username);

        await Task.WhenAll(getUsersTask, getUserCountTask);

        var users = await getUsersTask;
        var cnt = await getUserCountTask;

        var res = new GetUsersResponse
        {
            Users = users,
            Count = cnt
        };

        return res;
    }

    public async Task CreateUserAsync(CreateUserRequest req)
    {
        await _keycloakAdminRepository.CreateUserAsync(req.User);
    }

    public async Task ToggleUserEnabledAsync(string id) {
        await _keycloakAdminRepository.ToggleUserEnabledAsync(id);
    }

    public async Task ResetPasswordAsync(string id, CredentialDto credential) {
        await _keycloakAdminRepository.ResetPassword(id, credential);
    }

    public async Task<GetUserRolesResponse> GetUserRoleMappingsAsync(string id)
    {
        var client = await _keycloakAdminRepository.GetClientAsync();

        var userRolesTask = _keycloakAdminRepository.GetUserClientRolesAsync(id, client.Id);
        var availableRolesTask = _keycloakAdminRepository.GetAvailableClientRolesAsync(id, client.Id);

        await Task.WhenAll(userRolesTask, availableRolesTask);

        var userRoles = await userRolesTask;
        var availableRoles = await availableRolesTask;

        var userMappedRoles = userRoles.Select(r => { r.IsInRole = true; return r; })
            .Concat(availableRoles.Select(r => { r.IsInRole = false; return r; }))
            .OrderBy(r => r.Name)
            .ToList();

        var res = new GetUserRolesResponse(userMappedRoles);
        return res;
    }
}
