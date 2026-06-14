using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services.Interfaces;
using Hubly.api.Domain.Entities;

namespace Hubly.api.Pipeline
{
    public class TokenProcessor
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ICoWorkerRepository _coWorkerRepository;

        public TokenProcessor(
            ITokenService tokenService,
            IUserService userService,
            ICoWorkerRepository coWorkerRepository
            )
        {
            _tokenService = tokenService;
            _userService = userService;
            _coWorkerRepository = coWorkerRepository;
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
            if (!cookies.TryGetValue("token", out var token))
            {
                return null;
            }

            return await GetUserFromToken(token);
        }

        public async Task<(AuthenticatedUser owner, AuthenticatedUser? coWorker)?> ResolveOwnerAndCoWorker(AuthenticatedUser currentUser, bool hasAuthenticatedCoWorker)
        {
            if (!hasAuthenticatedCoWorker)
            {
                return (currentUser, null);
            }

            var coWorkerRelation = await _coWorkerRepository.GetCoWorker(currentUser.Id);
            if (coWorkerRelation == null)
            {
                return (currentUser, null);
            }

            var ownerResult = await _userService.GetUserInfo(coWorkerRelation.OwnerId);
            if (ownerResult.IsT1)
            {
                return null;
            }

            var owner = new AuthenticatedUser
            {
                Id = ownerResult.AsT0.Id,
                Token = currentUser.Token,
                Username = ownerResult.AsT0.Name,
                IsEmailConfirmed = ownerResult.AsT0.IsEmailConfirmed
            };

            var coWorker = new AuthenticatedUser
            {
                Id = currentUser.Id,
                Token = currentUser.Token,
                Username = currentUser.Username,
                IsEmailConfirmed = currentUser.IsEmailConfirmed
            };

            return (owner, coWorker);
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
                Username = userResult.AsT0.Name,
                IsEmailConfirmed = userResult.AsT0.IsEmailConfirmed
            };

        }
    }
}

