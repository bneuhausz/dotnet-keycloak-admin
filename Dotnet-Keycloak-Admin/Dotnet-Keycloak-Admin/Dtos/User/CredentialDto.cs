namespace Nbx.DotnetKeycloak.Admin.Dtos.User;

public record CredentialDto {
    public string Type { get; } = "password";
    public string Value { get; init; } = default!;
    public bool Temporary { get; } = true;
}
