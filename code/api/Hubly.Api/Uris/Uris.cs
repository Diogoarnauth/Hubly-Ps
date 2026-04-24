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
        public const string CheckCreatorOrCompany = $"{Prefix}/users/profile/CheckCreatorOrCompany";
        public const string Token = $"{Prefix}/users/token";
        public const string Logout = $"{Prefix}/users/logout";
        public const string EditUser = $"{Prefix}/users/edit";
        public const string EmailConfirmation = $"{Prefix}/users/emailConfirmation";
        public const string ResendEmailConfirmation = $"{Prefix}/users/resendEmailConfirmation";
        public const string VerifyEmail = $"{Prefix}/users/verifyEmail";
        public const string GetHistory = $"{Prefix}/users/getHistory";
        public const string FullCreatorProfile = $"{Prefix}/users/{{id:int}}/fullCreatorProfile";
        public const string FullCompanyProfile = $"{Prefix}/users/{{id:int}}/fullCompanyProfile";



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
        public const string AddSocialProfile = $"{Prefix}/creator/socialProfile";
        public const string RemoveSocialProfile = $"{Prefix}/creator/socialProfile/{{profileId:int}}";
        public const string GetSocialProfileById = $"{Prefix}/creator/socialProfile/{{profileId:int}}";
        public const string EditCreatorSocialProfile = $"{Prefix}/creator/socialProfile/edit/{{socialProfileId}}";
        public const string Search = $"{Prefix}/creators";
        public const string GetTrending = $"{Prefix}/creator/trending";

        //adicionar pesquisa com filtros e adicionar ver estatisticas sobre os chats e isso 



    }

    public static class Companies
    {

        public const string Create = $"{Prefix}/company";
        public const string GetById = $"{Prefix}/company/{{id}}";
        public const string EditCompanyProfile = $"{Prefix}/company/edit";
        public const string Search = $"{Prefix}/company";
        public const string GetTrending = $"{Prefix}/company/trending";


        //adicionar pesquisa com filtros e adicionar ver estatisticas sobre os chats e isso 



    }

    public static class SocialPlatforms
    {
        public const string GetAll = $"{Prefix}/socialPlatform";
    }

    public static class Sectors
    {
        public const string GetAll = $"{Prefix}/sectors";
    }
    public static class Countries
    {
        public const string GetCountries = $"{Prefix}/getCountries";

    }

    public static class Conversations
    {
        public const string Create = $"{Prefix}/conversation";
        public const string SendMessage = $"{Prefix}/conversation/{{conversationId}}/message";
        public const string EditMessage = $"{Prefix}/conversation/message/edit/{{messageId}}";
        public const string DeleteMessage = $"{Prefix}/conversation/message/{{messageId}}";
        public const string GetMessages = $"{Prefix}/conversation/{{conversationId}}/messages";

    }



}