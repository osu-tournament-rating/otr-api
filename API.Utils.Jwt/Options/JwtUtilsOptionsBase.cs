using System.Diagnostics.CodeAnalysis;
using CommandLine;
using Newtonsoft.Json;

namespace API.Utils.Jwt.Options;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public abstract class JwtUtilsOptionsBase
{
    /// <summary>
    /// Denotes the options as being valid
    /// </summary>
    [JsonIgnore]
    public bool IsValid { get; set; }

    [Option(
        'i',
        "issuer",
        Required = false,
        HelpText = "Issuer of the token, typically the domain of the API." +
                   "\nRequired from command line arg or from config file. " +
                   "Will attempt to populate from config file if not given." +
                   "\nExample: 'localhost:5075'"
    )]
    public string Issuer { get; set; } = string.Empty;

    [Option(
        'k',
        "key",
        Required = false,
        HelpText = "Signing key for the token, must be of at least 128 bits." +
                   "\nRequired from command line arg or from config file. " +
                   "Will attempt to populate from config file if not given. "
    )]
    public string Key { get; set; } = string.Empty;

    [Option(
        'a',
        "audience",
        Required = false,
        HelpText = "Audience of the token, typically the domain of the website." +
                   "\nRequired from command line arg or from config file. " +
                   "Will attempt to populate from config file if not given." +
                   "\nExample: 'localhost:3000'"
    )]
    public string Audience { get; set; } = string.Empty;

    [Option(
        'c',
        "config",
        Required = false,
        HelpText = "Path to an appsettings.json file containing a JWT configuration. " +
                   "Individual JWT configuration values supplied from the command line will take priority." +
                   "\nExample: '~/git/otr-api/API/appsettings.Development.json'"
    )]
    [JsonIgnore]
    public string ConfigFile { get; set; } = string.Empty;

    public override string ToString() =>
        JsonConvert.SerializeObject(this, Formatting.Indented);
}
