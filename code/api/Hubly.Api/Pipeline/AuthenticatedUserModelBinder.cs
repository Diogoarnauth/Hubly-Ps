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

            var httpContext = bindingContext.HttpContext;
            var value = httpContext.Items[_itemKey];

            if (value != null)
            {
                if (bindingContext.ModelType.IsInstanceOfType(value))
                {
                    bindingContext.Result = ModelBindingResult.Success(value);
                }
                else if (bindingContext.ModelType == typeof(AuthenticatedCoWorker) && value is AuthenticatedUser user)
                {
                    var coWorker = new AuthenticatedCoWorker
                    {
                        Id = user.Id,
                        Token = user.Token,
                        Username = user.Username,
                        IsEmailConfirmed = user.IsEmailConfirmed
                    };
                    bindingContext.Result = ModelBindingResult.Success(coWorker);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(null); 
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