using Hubly.api.Services.Interfaces;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Pipeline
{
    public class TokenProcessor
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public TokenProcessor(
            ITokenService tokenService,
            IUserService userService
            )
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<AuthenticatedUser?> ProcessAuthorizationHeader(string? authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var token = authorizationHeader.Substring(7).Trim();
            return await GetUserFromToken(token);
        }

        public async Task<AuthenticatedUser?> ProcessCookieToken(IRequestCookieCollection cookies)
        {
            if (!cookies.TryGetValue("auth_token", out var token))
            {
                return null;
            }

            return await GetUserFromToken(token);
        }

        private async Task<AuthenticatedUser?> GetUserFromToken(string token)
        {
            var userId = await _tokenService.ValidateToken(token, null);

            if (!userId.HasValue)
            {
                return null;
            }

            var userResult = await _userService.GetUserInfo(userId.Value);
            if (userResult.IsT1)
            {
                return null;
            }

            return new AuthenticatedUser
            {
                Id = userResult.AsT0.Id,
                Token = token,
                Username = userResult.AsT0.Name
            };
            
        }
    }
}

