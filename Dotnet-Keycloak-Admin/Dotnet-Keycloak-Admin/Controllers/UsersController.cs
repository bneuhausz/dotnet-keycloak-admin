﻿using Nbx.DotnetKeycloak.Admin.Requests;
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
    public async Task<CreatedResult> CreateUser([FromBody] CreateUserRequest req)
    {
        await _keycloakAdminService.CreateUserAsync(req);
        return Created();
    }
}
