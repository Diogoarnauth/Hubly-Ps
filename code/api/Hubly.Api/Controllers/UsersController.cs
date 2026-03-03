using Microsoft.AspNetCore.Mvc;
using Hubly.Domain.Entities;
using Hubly.api.DTOs;
using Hubly.Services;

namespace Hubly.api.Controllers;

[ApiController]

public class UserController : ControllerBase
{
    
    private readonly UsersDomain _usersDomain;

    public UserController(UsersDomain usersDomain)
    {
        _usersDomain = usersDomain;
    }

    [HttpPost(Uris.Uris.Users.Create)] //TODO() depois o Uris.Uris
    public async Task<IActionResult> Create([FromBody] UserCreateInputModel input)
    {
        var res = await _userService.Register (input.Name, input.Email, input.Password);
            
        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Register), success.Adapt<UserCreateOutputModel>()),
            error => error switch
            {
                UserError.InvalidPassword => ProblemResponse.InvalidPassword.ToResponse(),
                UserError.InvalidUsername => ProblemResponse.InvalidUsername.ToResponse(),
                UserError.EmailAlreadyInUse => ProblemResponse.EmailAlreadyInUse.ToResponse(),
                UserError.FailedToCreateUser => ProblemResponse.FailedToCreateUser.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
            

    }
}