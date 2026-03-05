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
                .Any(p => p.ParameterType == typeof(AuthenticatedUser));

            if (!hasAuthParameter)
            {
                return;
            }

            string? authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
            AuthenticatedUser? user = null;
            // Process the authorization header
            if (!string.IsNullOrEmpty(authHeader))
            {
                user = await _tokenProcessor.ProcessAuthorizationHeader(authHeader);
            }
            // Process the cookie token if the authorization header is not present
            if (user == null)
            {
                user = await _tokenProcessor.ProcessCookieToken(context.HttpContext.Request.Cookies);
            }
            // If the user is not authenticated, return a 401 Unauthorized status
            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                context.HttpContext.Response.Cookies.Delete("auth_token");
                context.HttpContext.Response.Headers.Append(
                    "WWW-Authenticate",
                    "Bearer");
                return;
            }

            context.HttpContext.Items["AuthenticatedUser"] = user;
        }
    }
}