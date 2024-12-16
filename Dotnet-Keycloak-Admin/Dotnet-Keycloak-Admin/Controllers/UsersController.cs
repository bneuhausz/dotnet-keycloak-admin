using Nbx.DotnetKeycloak.Admin.Requests;
using Nbx.DotnetKeycloak.Admin.Responses;
using Nbx.DotnetKeycloak.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nbx.DotnetKeycloak.Admin.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<GetUsersResponse>> GetUsers([FromQuery] GetUsersRequest req)
    {
        var res = await _keycloakAdminService.GetUsersAsync(req);
        return Ok(res);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest req)
    {
        await _keycloakAdminService.CreateUserAsync(req);
        return StatusCode(201);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ToggleUserEnabled(string id)
    {
        await _keycloakAdminService.ToggleUserEnabledAsync(id);
        return NoContent();
    }

    [HttpPut("{id}/reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest req)
    {
        await _keycloakAdminService.ResetPasswordAsync(id, req.Credential);
        return NoContent();
    }

    [HttpGet("{id}/roles")]
    public async Task<ActionResult<GetUserRolesResponse>> GetUserRoles(string id)
    {
        var res = await _keycloakAdminService.GetUserRoleMappingsAsync(id);
        return Ok(res);
    }

    [HttpPost("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> AssignRoleToUser(string id, [FromBody] AssignRoleRequest req)
    {
        await _keycloakAdminService.AssignRoleToUserAsync(id, req);
        return NoContent();
    }

    [HttpDelete("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RemoveRoleFromUser(string id, [FromBody] AssignRoleRequest req)
    {
        await _keycloakAdminService.RemoveRoleFromUserAsync(id, req);
        return NoContent();
    }
}
