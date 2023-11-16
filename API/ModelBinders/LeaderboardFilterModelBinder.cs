using API.DTOs;
using System.ComponentModel;
using System.Globalization;

namespace API.ModelBinders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

public class LeaderboardFilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var values = bindingContext.ValueProvider;

        // Create an instance of the FilterDTO
        var filterDto = new LeaderboardFilterDTO
        {
            MinRank = GetValue<int?>("MinRank"),
            MaxRank = GetValue<int?>("MaxRank"),
            MinRating = GetValue<int?>("MinRating"),
            MaxRating = GetValue<int?>("MaxRating"),
            MinMatches = GetValue<int?>("MinMatches"),
            MaxMatches = GetValue<int?>("MaxMatches"),
            MinWinrate = GetValue<double?>("MinWinrate"),
            MaxWinrate = GetValue<double?>("MaxWinrate"),
            TierFilters = new LeaderboardTierFilterDTO
            {
                FilterBronze = GetValue<bool?>("Bronze"),
                FilterSilver = GetValue<bool?>("Silver"),
                FilterGold = GetValue<bool?>("Gold"),
                FilterPlatinum = GetValue<bool?>("Platinum"),
                FilterDiamond = GetValue<bool?>("Diamond"),
                FilterMaster = GetValue<bool?>("Master"),
                FilterGrandmaster = GetValue<bool?>("Grandmaster"),
                FilterEliteGrandmaster = GetValue<bool?>("EliteGrandmaster"),
            }
        };

        // Set the result
        bindingContext.Result = ModelBindingResult.Success(filterDto);
        return Task.CompletedTask;

        // Helper function to retrieve values for the properties
        T? GetValue<T>(string key)
        {
            var valueProviderResult = values.GetValue(key);
            if (valueProviderResult != ValueProviderResult.None && !string.IsNullOrEmpty(valueProviderResult.FirstValue))
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertFrom(valueProviderResult.FirstValue.GetType()))
                {
                    return (T?)converter.ConvertFromString(null, CultureInfo.InvariantCulture, valueProviderResult.FirstValue);
                }
            }
            return default;
        }
    }
}
