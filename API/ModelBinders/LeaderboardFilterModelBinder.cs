using System.ComponentModel;
using System.Globalization;
using API.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.ModelBinders;
public class LeaderboardFilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        IValueProvider values = bindingContext.ValueProvider;

        // Create an instance of the FilterDTO
        var filterDto = new LeaderboardFilterDTO
        {
            MinRank = GetValue<int?>("MinRank"),
            MaxRank = GetValue<int?>("MaxRank"),
            MinRating = GetValue<int?>("MinRating"),
            MaxRating = GetValue<int?>("MaxRating"),
            MinMatches = GetValue<int?>("MinMatches"),
            MaxMatches = GetValue<int?>("MaxMatches"),
            MinWinRate = GetValue<double?>("MinWinrate"),
            MaxWinRate = GetValue<double?>("MaxWinrate"),
            TierFilters = new LeaderboardTierFilterDTO
            {
                FilterBronze = GetValue<bool>("Bronze"),
                FilterSilver = GetValue<bool>("Silver"),
                FilterGold = GetValue<bool>("Gold"),
                FilterPlatinum = GetValue<bool>("Platinum"),
                FilterEmerald = GetValue<bool>("Emerald"),
                FilterDiamond = GetValue<bool>("Diamond"),
                FilterMaster = GetValue<bool>("Master"),
                FilterGrandmaster = GetValue<bool>("Grandmaster"),
                FilterEliteGrandmaster = GetValue<bool>("EliteGrandmaster"),
            }
        };

        // Set the result
        bindingContext.Result = ModelBindingResult.Success(filterDto);
        return Task.CompletedTask;

        // Helper function to retrieve values for the properties
        T? GetValue<T>(string key)
        {
            ValueProviderResult valueProviderResult = values.GetValue(key);
            if (
                valueProviderResult != ValueProviderResult.None
                && !string.IsNullOrEmpty(valueProviderResult.FirstValue)
            )
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertFrom(valueProviderResult.FirstValue.GetType()))
                {
                    return (T?)
                        converter.ConvertFromString(
                            null,
                            CultureInfo.InvariantCulture,
                            valueProviderResult.FirstValue
                        );
                }
            }
            return default;
        }
    }
}
