using Dotnet_Keycloak_Admin.Dtos.User;
using Dotnet_Keycloak_Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Keycloak_Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IKeycloakAdminService _keycloakAdminService;

    public UsersController(IKeycloakAdminService keycloakAdminService)
    {
        _keycloakAdminService = keycloakAdminService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.Delay(1);
        return Ok("authorized");
    }

    [Authorize(Policy = "UserManagerPolicy")]
    [HttpGet("manage-users")]
    [ProducesResponseType<List<GetUserDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdmin()
    {
        var res = await _keycloakAdminService.GetUsersAsync();
        return Ok(res);
    }
}
