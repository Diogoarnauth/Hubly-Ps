using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Audit;

public class AuditLogFilter : ActionFilterAttribute
{
    private readonly string _actionName;

    public AuditLogFilter(string actionName)
    {
        _actionName = actionName;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        
        var statusCode = context.HttpContext.Response.StatusCode;

        if (/*resultContext.Result is ObjectResult objectResult &&*/ (statusCode >= 200 && statusCode < 300))
        {
            //(Queue) injetada via Dependency Injection
            var auditQueue = context.HttpContext.RequestServices.GetRequiredService<AuditQueue>();

            var user = context.HttpContext.Items["AuthenticatedUser"] as AuthenticatedUser;
            var coWorker = context.HttpContext.Items["AuthenticatedCoWorker"] as AuthenticatedCoWorker;

            var path = context.HttpContext.Request.Path;
            var payload = context.ActionArguments;

            await auditQueue.EnqueueAsync(new AuditLogEntry(
                _actionName, 
                user?.Id, 
                coWorker?.Id, 
                path, 
                payload
            ));
        }
    }
}