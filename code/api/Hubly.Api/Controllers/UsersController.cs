using Microsoft.AspNetCore.Mvc;
using Hubly.api.Domain.Entities;
using Hubly.api.DTOs;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Problems;
using Mapster; 

namespace Hubly.api.Controllers;

[ApiController]

public class UserController : ControllerBase
{
    
    private readonly UsersDomain _usersDomain;
    private readonly IUserService _userService;

    public UserController(UsersDomain usersDomain, IUserService userService)
    {
        _usersDomain = usersDomain;
        _userService = userService;
    }

    [HttpPost(Uris.Uris.Users.Create)] //TODO() depois o Uris.Uris
    public async Task<IActionResult> Create([FromBody] UserCreateInputModel input)
    {
        var res = await _userService.Register (input.Name, input.Email, input.Password);
            
        return res.Match<IActionResult>(
            success => CreatedAtAction(nameof(Create), success.Adapt<UserCreateOutputModel>()),
            error => error switch
            {
                UserError.InvalidPassword => ProblemResponse.InvalidPassword.ToResponse(),
                UserError.InvalidName => ProblemResponse.InvalidName.ToResponse(),
                UserError.EmailAlreadyExists => ProblemResponse.EmailAlreadyExists.ToResponse(),
                UserError.FailedUserCreation => ProblemResponse.FailedUserCreation.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
            

    }


[HttpPost(Uris.Uris.Users.Token)]
public async Task<IActionResult> Token([FromBody] UserCreateTokenInputModel input)
{
    var res = await _userService.Token(input.Email, input.Password);

    return res.Match<IActionResult>(
        success =>
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromHours(24)
            };

            Response.Cookies.Append("token", success.TokenValidation, cookieOptions);

            return Ok(success.Adapt<UserCreateTokenOutputModel>());
        },
        error => error switch
        {
            UserError.InvalidCredentials => ProblemResponse.InvalidCredentials.ToResponse(), 
            _ => ProblemResponse.InternalServerError.ToResponse()
        }
    );
}
}