using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hubly.api.Uris;

public static class Uris
{
    public const string Prefix = "/api";

    public static class Users
    {
        public const string Create = $"{Prefix}/users";
        public const string GetById = $"{Prefix}/users/{{id}}";
        public const string Token = $"{Prefix}/users/token";
        public const string Logout = $"{Prefix}/users/logout";
        public const string EditUser = $"{Prefix}/users/edit";

        public const string ChangePassword = $"{Prefix}/users/changePassword";
        // No C#, usamos string.Replace ou string.Format para expandir o ID
        public static string ById(int id) => GetById.Replace("{id}", id.ToString());

        public static string Register() => Create;

        public static string Login ()=> Token;

        public static string LogoutUser ()=> Logout;

        public static string PasswordChange ()=> ChangePassword;
    }
    
}