namespace Nbx.DotnetKeycloak.Admin.Dtos.Role;

public record GetRoleDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public bool IsInRole { get; set; }
}
