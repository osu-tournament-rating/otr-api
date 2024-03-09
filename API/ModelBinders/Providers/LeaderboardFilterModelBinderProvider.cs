using API.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace API.ModelBinders.Providers;

public class LeaderboardFilterModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType == typeof(LeaderboardFilterDTO))
        {
            return new BinderTypeModelBinder(typeof(LeaderboardFilterModelBinder));
        }

        return null;
    }
}
