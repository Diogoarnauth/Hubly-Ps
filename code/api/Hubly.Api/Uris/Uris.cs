using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hubly.api.Uris;

public static class Uris
{
    public const string Prefix = "/api";

    public static class Users
    {
        public const string Create = $"{Prefix}/users";
        public const string GetById = $"{Prefix}/users/{{id:int}}";
        public const string GetMyInfo = $"{Prefix}/users/profile/me";
        public const string Token = $"{Prefix}/users/token";
        public const string Logout = $"{Prefix}/users/logout";
        public const string EditUser = $"{Prefix}/users/edit";
        public const string EmailConfirmation = $"{Prefix}/users/emailConfirmation";
        public const string ResendEmailConfirmation = $"{Prefix}/users/resendEmailConfirmation";
        public const string VerifyEmail = $"{Prefix}/users/verifyEmail";


        public const string ChangePassword = $"{Prefix}/users/changePassword";
        // No C#, usamos string.Replace ou string.Format para expandir o ID
        public static string ById(int id) => GetById.Replace("{id}", id.ToString());

        public static string Register() => Create;

        public static string Login() => Token;

        public static string LogoutUser() => Logout;

        public static string PasswordChange() => ChangePassword;


    }

    public static class Creators
    {

        public const string Create = $"{Prefix}/creator";
        public const string GetById = $"{Prefix}/creator/{{id}}";
        public const string EditCreatorProfile = $"{Prefix}/creator/edit";
        public const string ChangeAvailabilityStatus = $"{Prefix}/creator/status";
        public const string RateCreator = $"{Prefix}/creator/rateCreator/{{id:int}}";


        //adicionar pesquisa com filtros e adicionar ver estatisticas sobre os chats e isso 



    }

    public static class Companies
    {

        public const string Create = $"{Prefix}/company";
        public const string GetById = $"{Prefix}/company/{{id}}";
        public const string EditCompanyProfile = $"{Prefix}/company/edit";


        //adicionar pesquisa com filtros e adicionar ver estatisticas sobre os chats e isso 



    }



}