using Nbx.DotnetKeycloak.Admin.Dtos.User;
using Nbx.DotnetKeycloak.Admin.Requests;
using Nbx.DotnetKeycloak.Admin.Responses;

namespace Nbx.DotnetKeycloak.Admin.Services.Interfaces;

public interface IKeycloakAdminService
{
    Task<GetUsersResponse> GetUsersAsync(GetUsersRequest req);
    Task CreateUserAsync(CreateUserRequest req);
}
