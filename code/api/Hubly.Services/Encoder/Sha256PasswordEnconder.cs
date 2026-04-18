using BCrypt.Net;
using Hubly.api.Services.Interfaces;

namespace Hubly.api.Services.Encoder
{
    public class BCryptPasswordEncoder : IPasswordEncoder
    {
        public string createValidationInformation(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}