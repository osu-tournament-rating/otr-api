using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace API.Utilities.DataAnnotations;

public class SupportedRulesetAttribute : AllowedValuesAttribute
{
    public SupportedRulesetAttribute() : base(Ruleset.Osu, Ruleset.Taiko, Ruleset.Catch, Ruleset.Mania4k, Ruleset.Mania7k)
    {
        ErrorMessage = "Ruleset not supported. Supported rulesets are (0 = osu!, 1 = osu!Taiko, 2 = osu!Catch, 4 = osu!mania 4K, 5 = osu!mania 7K)";
    }
}
