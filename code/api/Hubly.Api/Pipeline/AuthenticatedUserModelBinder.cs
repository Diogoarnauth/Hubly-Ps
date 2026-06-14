using Hubly.api.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hubly.api.Pipeline
{
    public class AuthenticatedUserModelBinder : IModelBinder
    {
        private readonly string _itemKey;

        public AuthenticatedUserModelBinder(string itemKey)
        {
            _itemKey = itemKey;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            HttpContext httpContext = bindingContext.HttpContext;
            var value = httpContext.Items[_itemKey];

            if (value != null)
            {
                bindingContext.Result = ModelBindingResult.Success(value);
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
                return new AuthenticatedUserModelBinder("AuthenticatedUser");
            }

            if (context.Metadata.ModelType == typeof(AuthenticatedCoWorker))
            {
                return new AuthenticatedUserModelBinder("AuthenticatedCoWorker");
            }

            return null;
        }
    }

}