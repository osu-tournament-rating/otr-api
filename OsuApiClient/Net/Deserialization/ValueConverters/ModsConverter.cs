using AutoMapper;
using Common.Enums;

namespace OsuApiClient.Net.Deserialization.ValueConverters;

/// <summary>
/// Converts a list of strings into its respective <see cref="Mods"/>
/// </summary>
public class ModsConverter : IValueConverter<IEnumerable<string>, Mods>
{
    public Mods Convert(IEnumerable<string> sourceMember, ResolutionContext context) =>
        Convert(sourceMember);

    public static Mods Convert(IEnumerable<string> value)
    {
        Mods outMods = Mods.None;

        foreach (var stringMod in value)
        {
            switch (stringMod)
            {
                case "NF": outMods |= Mods.NoFail; break;
                case "EZ": outMods |= Mods.Easy; break;
                case "TD": outMods |= Mods.TouchDevice; break;
                case "HD": outMods |= Mods.Hidden; break;
                case "HR": outMods |= Mods.HardRock; break;
                case "SD": outMods |= Mods.SuddenDeath; break;
                case "DT": outMods |= Mods.DoubleTime; break;
                case "RX": outMods |= Mods.Relax; break;
                case "HT": outMods |= Mods.HalfTime; break;
                case "NC": outMods |= Mods.Nightcore; break;
                case "FL": outMods |= Mods.Flashlight; break;
                case "AT": outMods |= Mods.Autoplay; break;
                case "SO": outMods |= Mods.SpunOut; break;
                case "AP": outMods |= Mods.Autoplay; break;
                case "PF": outMods |= Mods.Perfect; break;
                case "4K": outMods |= Mods.Key4; break;
                case "5K": outMods |= Mods.Key5; break;
                case "6K": outMods |= Mods.Key8; break;
                case "7K": outMods |= Mods.Key7; break;
                case "8K": outMods |= Mods.Key8; break;
                case "FI": outMods |= Mods.FadeIn; break;
                case "RD": outMods |= Mods.Random; break;
                case "CM": outMods |= Mods.Cinema; break;
                case "TP": outMods |= Mods.Target; break;
                case "9K": outMods |= Mods.Key9; break;
                case "CO": outMods |= Mods.KeyCoop; break;
                case "1K": outMods |= Mods.Key1; break;
                case "3K": outMods |= Mods.Key3; break;
                case "2K": outMods |= Mods.Key2; break;
                case "MR": outMods |= Mods.Mirror; break;
            }
        }

        return outMods;
    }
}
