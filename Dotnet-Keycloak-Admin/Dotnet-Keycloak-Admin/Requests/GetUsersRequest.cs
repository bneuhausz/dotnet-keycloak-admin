namespace Nbx.DotnetKeycloak.Admin.Requests;

public class GetUsersRequest
{
    public int First { get; set; }
    public int Max { get; set; } = 10;
    public string Username { get; set; } = string.Empty;
}
