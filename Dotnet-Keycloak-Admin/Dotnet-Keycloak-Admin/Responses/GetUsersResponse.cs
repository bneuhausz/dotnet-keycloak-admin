using Nbx.DotnetKeycloak.Admin.Dtos.User;

namespace Nbx.DotnetKeycloak.Admin.Responses;

public class GetUsersResponse
{
    public int Count { get; set; }
    public List<GetUserDto> Users { get; set; } = [];
}
