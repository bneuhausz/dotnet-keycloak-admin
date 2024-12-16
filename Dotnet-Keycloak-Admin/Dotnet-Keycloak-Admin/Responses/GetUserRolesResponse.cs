using Nbx.DotnetKeycloak.Admin.Dtos.Role;

namespace Nbx.DotnetKeycloak.Admin.Responses;

public record GetUserRolesResponse(List<GetRoleDto> Roles);
