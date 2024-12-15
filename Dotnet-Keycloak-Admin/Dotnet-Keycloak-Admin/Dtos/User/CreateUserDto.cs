namespace Nbx.DotnetKeycloak.Admin.Dtos.User;

public record CreateUserDto
{
    public string Username { get; init; } = default!;
    public bool Enabled { get; init; }
    public Dictionary<string, string> Attributes { get; } = new()
    {
        { "type", "regular" }
    };
}
