using Hubly.api.Domain.Entities;
using Hubly.api.DTOs;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Problems;
using Hubly.api.Pipeline;
using Mapster; 
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost(Uris.Uris.Users.Create)] 
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

            Response.Cookies.Append("token", success, cookieOptions);

           return Ok(new UserCreateTokenOutputModel 
            { 
                Token = success 
            });
},
        error => error switch
        {
            UserError.InvalidCredentials => ProblemResponse.InvalidCredentials.ToResponse(), 
            _ => ProblemResponse.InternalServerError.ToResponse()
        }
    );
}

[HttpPost(Uris.Uris.Users.Logout)]
public async Task<IActionResult> Logout(AuthenticatedUser user)
    {
        var response = await _userService.Logout(user.Token);
        if (Request.Cookies.ContainsKey("auth_token"))
        {
            Response.Cookies.Delete("auth_token");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Append("auth_token", "", cookieOptions);
        }
        return response.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                UserError.FailedToLogout => ProblemResponse.FailedToLogout.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Users.GetById)]
    public async Task<IActionResult> GetUserInfo(AuthenticatedUser user)
    {
        var response = await _userService.GetUserInfo(user.Id);
        return response.Match<IActionResult>(
            success => Ok(success.Adapt<UserInfoOutputModel>()),
            error => error switch
            {
                UserError.FailedToGetUserInfo => ProblemResponse.FailedToGetUserInfo.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPatch(Uris.Uris.Users.EditUser)]
    public async Task<IActionResult> EditUser( [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] EditUserInputModel request)// TODO() prof
    {
        var response = await _userService.EditUser(user.Id, request.NewUsername);
        return response.Match<IActionResult>(
            success => Ok(new EditUserInputModel { NewUsername = request.NewUsername }),
            error => error switch
            {
                UserError.FailedToEditUser => ProblemResponse.FailedToEditUser.ToResponse(),
                _=> ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

}