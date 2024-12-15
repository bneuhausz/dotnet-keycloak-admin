using Dotnet_Keycloak_Admin.Dtos.User;
using Dotnet_Keycloak_Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Keycloak_Admin.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserManagerPolicy")]
public class UsersController : ControllerBase
{
    private readonly IKeycloakAdminService _keycloakAdminService;

    public UsersController(IKeycloakAdminService keycloakAdminService)
    {
        _keycloakAdminService = keycloakAdminService;
    }

    [HttpGet("count")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Count()
    {
        var res = await _keycloakAdminService.GetUserCountAsync();
        return Ok(res);
    }

    [HttpGet]
    [ProducesResponseType<List<GetUserDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdmin()
    {
        var res = await _keycloakAdminService.GetUsersAsync();
        return Ok(res);
    }
}
