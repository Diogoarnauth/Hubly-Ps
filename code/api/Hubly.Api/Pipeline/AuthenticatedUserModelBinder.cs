using Hubly.api.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hubly.api.Pipeline
{
    public class AuthenticatedUserModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            HttpContext httpContext = bindingContext.HttpContext;
            AuthenticatedUser? user = httpContext.Items["AuthenticatedUser"] as AuthenticatedUser;

            if (user != null)
            {
                bindingContext.Result = ModelBindingResult.Success(user);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }

    public class AuthenticatedUserModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(AuthenticatedUser))
            {
                return new AuthenticatedUserModelBinder();
            }

            return null;
        }
    }

}