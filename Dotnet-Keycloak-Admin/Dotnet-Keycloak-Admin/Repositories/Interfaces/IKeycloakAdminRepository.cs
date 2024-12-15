using Nbx.DotnetKeycloak.Admin.Dtos.User;

namespace Nbx.DotnetKeycloak.Admin.Repositories.Interfaces;

public interface IKeycloakAdminRepository
{
    Task<int> GetUserCountAsync(string username);
    Task<List<GetUserDto>> GetUsersAsync(int first, int max, string username);
    Task CreateUserAsync(CreateUserDto user);
}
