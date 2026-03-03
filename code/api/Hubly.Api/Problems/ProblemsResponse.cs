using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

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


}