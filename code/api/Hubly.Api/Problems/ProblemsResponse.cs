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

    public static readonly ProblemResponse FailedToUpdateStatus = new (
        "failed-to-update-status",
        "Failed To Update Status",
    400);

    public static readonly ProblemResponse CreatorNotFound = new (
        "creator-not-found",
        "Creator Not Found",
    400);
    public static readonly ProblemResponse InvalidRating = new (
        "invalid-rating",
        "Invalid Rating",
    400);
    public static readonly ProblemResponse ErrorRatingCreator = new (
        "error rating creator",
        "Error Rating Creator",
    400);  
     public static readonly ProblemResponse SelfRatingNotAllowed = new (
        "self-rating-not-allowed",
        "Self Rating Not Allowed",
    400); 
    

}