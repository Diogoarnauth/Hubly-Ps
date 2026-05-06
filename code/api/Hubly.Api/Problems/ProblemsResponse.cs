using Microsoft.AspNetCore.Mvc;

namespace Hubly.api.Problems;

public class ProblemResponse
{

    private const string BASE_URL = "https://github.com/Diogoarnauth/Hubly-Ps/";
    private const string MEDIA_TYPE = "application/problem+json";

    public string Type { get; }
    public string Message { get; }
    public int Status { get; }

    private ProblemResponse(string type, string message, int status) // private constuctor 
    {
        Type = $"{BASE_URL}{type}";
        Message = message;
        Status = status;
    }

    public IActionResult ToResponse()//todo penso que seja a fun de criar o json completo(ver depois )
    {
        return new ObjectResult(this)
        {
            StatusCode = Status,
            ContentTypes = { MEDIA_TYPE },
        };
    }

    public IActionResult ToResponseWithLocation(string location)
    {
        return new ProblemWithLocationResult(this, location);
    }

    private class ProblemWithLocationResult : IActionResult
    {
        private readonly ProblemResponse _problem;
        private readonly string _location;

        public ProblemWithLocationResult(ProblemResponse problem, string location)
        {
            _problem = problem;
            _location = location;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.Headers.Location = _location;

            var objectResult = new ObjectResult(_problem)
            {
                StatusCode = _problem.Status,
                ContentTypes = { MEDIA_TYPE }
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }

    public static readonly ProblemResponse FailedUserCreation = new(
        "failed-to-create-user",
        "Failed to create User",
        500);

    public static readonly ProblemResponse InvalidName = new(
        "invalid-name",
        "The provided name is invalid",
        400);

    public static readonly ProblemResponse InvalidEmail = new(
        "invalid-email",
        "The provided email is invalid",
        400);

    public static readonly ProblemResponse EmailAlreadyExists = new(
        "email-already-exists",
        "There is already an account registered on that email",
        400);

    public static readonly ProblemResponse InvalidPassword = new(
        "invalid-password",
        "The password provided is invalid",
        400);

    public static readonly ProblemResponse InternalServerError = new(
        "internal-server-error",
        "InternalServerError",
        500);

    public static readonly ProblemResponse FailedToGetUserInfo = new(
        "failed-to-get-user-info",
        "Failed to get user information",
        404);

    public static readonly ProblemResponse InvalidCredentials = new(
        "invalid-credentials",
        "Invalid Credentials (email or password)",
        409);

    public static readonly ProblemResponse FailedToLogout = new(
        "failed-to-logout",
        "Failed To Logout",
    400);

    public static readonly ProblemResponse FailedToEditUser = new(
        "failed-to-edit-user",
        "Failed To Edit User",
    400);
    public static readonly ProblemResponse UserNotFound = new(
    "user-not-found",
    "User Not Found",
    404
    );

    public static readonly ProblemResponse OldPasswordIsIncorrect = new(
        "old-password-is-incorrect",
        "Old Password Is Incorrect",
    400);
    public static readonly ProblemResponse NewPasswordCannotBeTheSameAsTheOldPassword = new(
        "new-password-cannot-be-the-same-as-the-old-password",
        "New Password Cannot Be The Same As The Old Password",
    400);
    public static readonly ProblemResponse InvalidConfirmationCode = new(
        "invalid-confirmation-code",
        "The provided confirmation code is invalid",
    400);

    public static readonly ProblemResponse FailedToConfirmEmail = new(
        "failed-to-confirm-email",
        "Failed to confirm email",
    500);

    public static readonly ProblemResponse EmailAlreadyConfirmed = new(
        "email-already-confirmed",
        "Email already confirmed",
    400);
    public static readonly ProblemResponse CodeAlreadyExists = new(
        "code-already-exists",
        "Code already exists",
    400);
    public static readonly ProblemResponse CreatorAlreadyExists = new(
        "creator-already-exists",
        "Creator Already Exists",
    400);
    public static readonly ProblemResponse UserAlreadyRegisteredAsCompany = new(
            "user-already-registered-as-company",
            "User Already Registered As Company",
        409);

    public static readonly ProblemResponse InvalidArtisticName = new(
            "invalid-artistic-name",
            "Invalid Artistic Name",
        400);

    public static readonly ProblemResponse CompanyAlreadyExists = new(
        "company-already-exists",
        "Company Already Exists",
    400);

    public static readonly ProblemResponse UserAlreadyRegisteredAsCreator = new(
        "user-already-registered-as-creator",
        "User Already Registered As Creator",
    409);

    public static readonly ProblemResponse InvalidStatus = new(
        "invalid-status",
        "Invalid Status",
    400);

    public static readonly ProblemResponse FailedToUpdateStatus = new(
        "failed-to-update-status",
        "Failed To Update Status",
    400);

    public static readonly ProblemResponse CreatorNotFound = new(
        "creator-not-found",
        "Creator Not Found",
    404);
    public static readonly ProblemResponse InvalidRating = new(
        "invalid-rating",
        "Invalid Rating",
    400);
    public static readonly ProblemResponse ErrorRatingCreator = new(
        "error rating creator",
        "Error Rating Creator",
    400);
    public static readonly ProblemResponse SelfRatingNotAllowed = new(
       "self-rating-not-allowed",
       "Self Rating Not Allowed",
   400);
    public static readonly ProblemResponse CompanyNotFound = new(
        "company-not-found",
        "Company Not Found",
    404);
    public static readonly ProblemResponse InvalidWebSiteLink = new(
            "invalid-web-site-link",
            "Invalid Web Site Link",
        404);
    public static readonly ProblemResponse InvalidCountryHeadquarters = new(
        "invalid-country-headquarters",
        "Invalid Country Headquarters",
    400);
    public static readonly ProblemResponse FailedToGetCompanyInfo = new(
        "invalid-country-headquarters",
        "Invalid Country Headquarters",
    500);
 
     public static readonly ProblemResponse InvalidPriceRange = new(
       "invalid-price-range",
       "Invalid Price Range",
   401);

    public static readonly ProblemResponse PlatformNotFound = new(
        "platform-not-found",
        "Platform Not Found",
    404);

    public static readonly ProblemResponse SocialProfileAlreadyExists = new(
        "social-profile-already-exists",
        "Social Profile Already Exists",
    409);

    public static readonly ProblemResponse FailedToGetCreatorSocialProfileInfo = new(
        "failed-to-get-platforms",
        "Failed To Get Platforms",
    404);

    public static readonly ProblemResponse FailedToGetPlatforms = new(
        "failed-to-get-platforms",
        "Failed To Get Platforms",
    404);
    public static readonly ProblemResponse SocialProfileNotFound = new(
        "social-profile-not-found",
        "Social Profile Not Found",
    404);

    public static readonly ProblemResponse InvalidFollowersCount = new(
           "invalid-followers-count",
           "Invalid Followers Count",
       401);

    public static readonly ProblemResponse ProfileDoesntBellongToYou = new(
              "profile-doesnt-bellong-to-you",
              "Profile Doesnt Bellong To You",
          403);
    public static readonly ProblemResponse InvalidSectorName = new(
        "invalid-sector-name",
        "Invalid Sector Name",
    404);

    public static readonly ProblemResponse FailedToGetCreatorInfo = new(
        "failed-to-get-creator-info",
        "Failed To Get Creator Info",
    404);

    public static readonly ProblemResponse SectorsNotFound = new(
        "sectors-not-found",
        "Sectors Not Found",
    404);
    public static readonly ProblemResponse ConversationAlreadyExists = new(
        "conversation-already-exists",
        "Conversation Already Exists",
    409);
    public static readonly ProblemResponse InvalidParticipantRole = new(
        "invalid-participant-role",
        "Invalid Participant Role",
    409);
    public static readonly ProblemResponse AccessDenied = new(
        "access-denied",
        "Access Denied",
    403);
    public static readonly ProblemResponse MessageNotFound = new(
        "message-not-found",
        "Message Not Found",
    404);
    public static readonly ProblemResponse MessageAlreadyDeleted = new(
        "message-already-deleted",
        "Message Already Deleted",
    404);
    public static readonly ProblemResponse TagNotFound = new(
        "tag-not-found",
        "Tag Not Found",
    404);
    public static readonly ProblemResponse TagNameAlreadyExists = new(
        "tag-name-already-exists",
        "Tag Name Already Exists",
    409);
    public static readonly ProblemResponse InvalidTagName = new(
        "invalid-tag-name",
        "Invalid Tag Name",
    400);
    public static readonly ProblemResponse InvalidColorHex = new(
        "invalid-color-hex",
        "Invalid Color Hex",
    400);
    public static readonly ProblemResponse ConversationNotFound = new(
        "conversation-not-found",
        "Conversation Not Found",
    404);
    public static readonly ProblemResponse UnauthorizedAccess = new(
        "unauthorized-access",
        "Unauthorized Access",
    403);
}