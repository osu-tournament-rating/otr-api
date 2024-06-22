// using Database.Entities;
//
// namespace API.Osu.AutomationChecks;
//
// public static class ScoreAutomationChecks
// {
//     public static bool PassesAutomationChecks(GameScore score) =>
//         PassesValueCheck(score) && PassesModsCheck(score);
//
//     public static bool PassesModsCheck(GameScore score)
//     {
//         if (score.EnabledMods == null)
//         {
//             return true;
//         }
//
//         return !AutomationConstants.UnallowedMods.Any(unallowedMod =>
//             score.EnabledModsEnum!.Value.HasFlag(unallowedMod)
//         );
//     }
//
//     public static bool PassesValueCheck(GameScore score) => score.Score > 1000;
// }
