using Hubly.api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hubly.api.Pipeline
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireAuthenticationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly TokenProcessor _tokenProcessor;

        public RequireAuthenticationAttribute(TokenProcessor tokenProcessor)
        {
            _tokenProcessor = tokenProcessor;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var hasAuthParameter = context.ActionDescriptor.Parameters
                .Any(p => p.ParameterType == typeof(AuthenticatedUser) || p.ParameterType == typeof(AuthenticatedCoWorker));

            if (!hasAuthParameter)
            {
                return;
            }

            var hasAuthCoWorker = context.ActionDescriptor.Parameters
                .Any(p => p.ParameterType == typeof(AuthenticatedCoWorker));

            string? authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
            AuthenticatedUser? currentUser = null;
            // Process the authorization header
            if (!string.IsNullOrEmpty(authHeader))
            {
                currentUser = await _tokenProcessor.ProcessAuthorizationHeader(authHeader);
            }
            // Process the cookie token if the authorization header is not present
            if (currentUser == null)
            {
                currentUser = await _tokenProcessor.ProcessCookieToken(context.HttpContext.Request.Cookies);
            }
            // If the user is not authenticated, return a 401 Unauthorized status
            if (currentUser == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                context.HttpContext.Response.Cookies.Delete("token");
                context.HttpContext.Response.Headers.Append(
                    "WWW-Authenticate",
                    "Bearer");
                return;
            }
            if (!currentUser.IsEmailConfirmed)
            {
                context.Result = new ObjectResult(new
                {
                    message = "Email confirmation required to perform this action.",
                    code = "EMAIL_NOT_CONFIRMED"
                })
                { StatusCode = 403 };
                return;
            }
            Console.WriteLine($"Authenticated user ID: {currentUser.Id}, Username: {currentUser.Username}, hasAuthCoWorker: {hasAuthCoWorker}");
            var resolved = await _tokenProcessor.ResolveOwnerAndCoWorker(currentUser, hasAuthCoWorker);
            if (resolved == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                context.HttpContext.Response.Cookies.Delete("token");
                context.HttpContext.Response.Headers.Append(
                    "WWW-Authenticate",
                    "Bearer");
                return;
            }

            context.HttpContext.Items["AuthenticatedUser"] = resolved.Value.owner;
            context.HttpContext.Items["AuthenticatedCoWorker"] = resolved.Value.coWorker;
        }
    }
}