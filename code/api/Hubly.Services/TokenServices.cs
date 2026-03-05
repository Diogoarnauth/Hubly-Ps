using System.Security.Cryptography;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Hubly.api.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Hubly.api.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenEncoder _tokenEncoder;
        private readonly int _tokenSizeInBytes;
        private readonly int _tokenExpirationTime;
        private readonly ITokenRepository _tokenRepository;
        public TokenService(
            ITokenEncoder tokenEncoder,
            IConfiguration configuration,
            ITokenRepository tokenRepository)
        {
            _tokenEncoder = tokenEncoder;
            _tokenSizeInBytes = int.Parse(configuration.GetSection("TokenSettings:SizeInBytes").Value ?? "32");
            _tokenExpirationTime = int.Parse(configuration.GetSection("TokenSettings:ExpirationTime").Value ?? "3600");
            _tokenRepository = tokenRepository;
        }

        public string GenerateTokenValue()
        {
            var randomBytes = new byte[_tokenSizeInBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public string CreateTokenValidationInformation(string token)
        {
            return _tokenEncoder.CreateValidationInformation(token);
        }

        public async Task<bool> CreateToken(int userId, string validationInformation, ITransactionContext context)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var newToken = new Token
            {
                TokenValidation = validationInformation,
                UserId = userId,
                CreatedAt = currentTime,
                LastUsedAt = currentTime
            };
            return await context.TokenRepository.CreateToken(newToken);
        }

        public async Task<int?> ValidateToken(string tokenValue, ITransactionContext? context)
        {
            var tokenValidation = CreateTokenValidationInformation(tokenValue);
            var token = await (context == null ? _tokenRepository.GetTokenByValidation(tokenValidation) : context.TokenRepository.GetTokenByValidation(tokenValidation));
            if (token == null)
            {
                return null;
            }
            if (token.CreatedAt + _tokenExpirationTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                await (context == null ? _tokenRepository.DeleteTokenByValidation(token.TokenValidation) : context.TokenRepository.DeleteTokenByValidation(token.TokenValidation));
                return null;
            }
            await (context == null ? _tokenRepository.RefreshToken(token.TokenValidation) : context.TokenRepository.RefreshToken(token.TokenValidation));
            return token.UserId;
        }

        public async Task<bool> DeleteToken(string tokenValue, ITransactionContext? context)
        {
            var tokenValidation = CreateTokenValidationInformation(tokenValue);
            if (context == null)
            {
                return await _tokenRepository.DeleteTokenByValidation(tokenValidation);
            }
            return await context.TokenRepository.DeleteTokenByValidation(tokenValidation);
        }
    }
}
