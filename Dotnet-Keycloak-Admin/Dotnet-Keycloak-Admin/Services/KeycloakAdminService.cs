using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;
using Nbx.DotnetKeycloak.Admin.Requests;
using Nbx.DotnetKeycloak.Admin.Responses;
using Nbx.DotnetKeycloak.Admin.Services.Interfaces;

namespace Nbx.DotnetKeycloak.Admin.Services;

public class KeycloakAdminService : IKeycloakAdminService
{
    private readonly IKeycloakAdminRepository _keycloakAdminRepository;

    public KeycloakAdminService(IKeycloakAdminRepository keycloakAdminRepository)
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

    public async Task ResetPassword(string id, CredentialDto credential) {
        await _keycloakAdminRepository.ResetPassword(id, credential);
    }
}
