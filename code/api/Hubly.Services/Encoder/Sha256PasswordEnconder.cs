using System.Security.Cryptography;
using System.Text;
using Hubly.api.Services.Interfaces;

namespace Hubly.api.Services.Encoder
{
    public class Sha256PasswordEncoder : IPasswordEncoder
    {
        public string createValidationInformation(string password)
        {
            return Hash(password);
        }

        private string Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
