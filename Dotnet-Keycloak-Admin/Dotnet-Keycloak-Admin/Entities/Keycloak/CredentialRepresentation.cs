namespace Nbx.DotnetKeycloak.Admin.Entities.Keycloak;

public record CredentialRepresentation
{
    public required string Type { get; init; }
    public required string Value { get; init; }
    public bool Temporary { get; init; }
}
