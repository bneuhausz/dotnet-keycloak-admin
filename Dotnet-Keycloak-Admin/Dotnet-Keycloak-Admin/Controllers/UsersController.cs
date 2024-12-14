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
        var token = await _keycloakAdminService.Test();
        return Ok(token);
    }
}
