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
        var res = await _userService.Register(input.Name, input.Email, input.Password);

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
        if (Request.Cookies.ContainsKey("token"))
        {
            Response.Cookies.Delete("token");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(-1)
            };
            Response.Cookies.Append("token", "", cookieOptions);
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
    public async Task<IActionResult> GetUserInfo([FromRoute] int id)
    {
        var response = await _userService.GetUserInfo(id);
        return response.Match<IActionResult>(
            success => Ok(MapToUserInfo(success)),
            error => error switch
            {
                UserError.FailedToGetUserInfo => ProblemResponse.FailedToGetUserInfo.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Users.GetMyInfo)]
    public async Task<IActionResult> GetMyInfo(AuthenticatedUser user)
    {
        var response = await _userService.GetUserInfo(user.Id);
        return response.Match<IActionResult>(
            success => Ok(MapToUserInfo(success)),
            error => error switch
            {
                UserError.FailedToGetUserInfo => ProblemResponse.FailedToGetUserInfo.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Users.CheckCreatorOrCompany)]
    public async Task<IActionResult> CheckCreatorOrCompany([FromServices] AuthenticatedUser user)
    {
        var response = await _userService.CheckCreatorOrCompany(user.Id);

        return response.Match<IActionResult>(
            hasProfile => Ok(new { hasProfile }), // Retorna um objeto simples { "hasProfile": true }
            error => error switch
            {
                UserError.FailedToGetUserInfo => ProblemResponse.FailedToGetUserInfo.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Users.EditUser)]
    public async Task<IActionResult> EditUser([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] EditUserInputModel request)// TODO() prof
    {
        var response = await _userService.EditUser(user.Id, request.NewUsername);
        return response.Match<IActionResult>(
            success => Ok(new EditUserInputModel { NewUsername = request.NewUsername }),
            error => error switch
            {
                UserError.FailedToEditUser => ProblemResponse.FailedToEditUser.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Users.ChangePassword)]
    public async Task<IActionResult> ChangePassword([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromBody] ChangePasswordInputModel request)
    {
        var response = await _userService.ChangePassword(user.Id, request.OldPassword, request.NewPassword);
        return response.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                UserError.InvalidPassword => ProblemResponse.InvalidPassword.ToResponse(),
                UserError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                UserError.OldPasswordIsIncorrect => ProblemResponse.OldPasswordIsIncorrect.ToResponse(),
                UserError.NewPasswordCannotBeTheSameAsTheOldPassword => ProblemResponse.NewPasswordCannotBeTheSameAsTheOldPassword.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpPost(Uris.Uris.Users.VerifyEmail)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailInputModel request)
    {
        var result = await _userService.VerifyConfirmationCodeAsync(request.Email, request.Code);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                UserError.InvalidConfirmationCode => ProblemResponse.InvalidConfirmationCode.ToResponse(),
                UserError.FailedToConfirmEmail => ProblemResponse.FailedToConfirmEmail.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }
    
    
    [HttpPost(Uris.Uris.Users.ResendEmailConfirmation)]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationInputModel request)
    {
        var response = await _userService.ResendEmailConfirmation(request.Email);
        return response.Match<IActionResult>(
            success => NoContent(),
            error => error switch
            {
                UserError.EmailAlreadyConfirmed => ProblemResponse.EmailAlreadyConfirmed.ToResponse(),
                UserError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                UserError.CodeAlreadyExists => ProblemResponse.CodeAlreadyExists.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }



    [HttpGet(Uris.Uris.Users.GetHistory)]
    public async Task<IActionResult> GetHistory(
        [ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var res = await _userService.GetHistory(user.Id, page, pageSize);

        return res.Match<IActionResult>(
            success =>
            {
                var response = new PagedResponse<ProfileHistoryOutputModel>
                {
                    Page = success.Page,
                    PageSize = success.PageSize,
                    TotalItems = success.TotalItems,
                    Items = success.Items.Select(h => new ProfileHistoryOutputModel
                    {
                        Id = h.Id,
                        ViewedAt = h.ViewedAt,
                        TargetId = h.ViewedCompanyId ?? h.ViewedSocialProfileId ?? 0,
                        TargetType = h.ViewedCompanyId.HasValue ? "Company" : "CreatorSocialProfile",
                        TargetName = h.ViewedCompanyId.HasValue
                            ? (h.ViewedCompany?.CompanyName ?? "Empresa Desconhecida")
                            : (h.ViewedSocialProfile?.PlatformUserName ?? "Criador Desconhecido")
                    }).ToList()
                };

                return Ok(response);
            },
            error => ProblemResponse.InternalServerError.ToResponse()
        );
    }

    [HttpGet(Uris.Uris.Users.FullCreatorProfile)]
    public async Task<IActionResult> GetFullCreatorProfile([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromRoute] int id)
    {
        var result = await _userService.GetFullCreatorProfile(id, user.Id);

        return result.Match<IActionResult>(
            success =>
            {
                var output = success.Adapt<FullUserProfileOutputModel>();
                output.IsOwner = (id == user.Id);
                return Ok(output);
            },
            error => error switch
            {
                UserError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }

    [HttpGet(Uris.Uris.Users.FullCompanyProfile)]
    public async Task<IActionResult> GetFullCompanyProfile([ModelBinder(typeof(AuthenticatedUserModelBinder))] AuthenticatedUser user, [FromRoute] int id)
    {
        var result = await _userService.GetFullCompanyProfile(id, user.Id);

        return result.Match<IActionResult>(
            success =>
            {
                var output = success.Adapt<FullCompanyProfileOutputModel>();
                output.IsOwner = (id == user.Id);
                return Ok(output);
            },
            error => error switch
            {
                UserError.UserNotFound => ProblemResponse.UserNotFound.ToResponse(),
                _ => ProblemResponse.InternalServerError.ToResponse()
            }
        );
    }



    private UserInfoOutputModel MapToUserInfo(User user)
    {
        var model = user.Adapt<UserInfoOutputModel>();

        if (user.Creator != null)
        {
            model.Role = "creator";
        }
        else if (user.Company != null)
        {
            model.Role = "company";
        }
        else
        {
            model.Role = null;
        }

        return model;
    }

}