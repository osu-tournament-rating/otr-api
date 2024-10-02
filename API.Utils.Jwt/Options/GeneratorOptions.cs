using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using API.Authorization;
using CommandLine;
using Newtonsoft.Json;

namespace API.Utils.Jwt.Options;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[Verb("generate", isDefault: true, HelpText = "Generate a new JWT")]
public class GeneratorOptions : IJwtUtilsOptions
{
    /// <summary>
    /// Denotes the options as being valid
    /// </summary>
    [JsonIgnore]
    public bool IsValid { get; set; }

    [Option(
        "subject",
        Required = true,
        HelpText = "Subject of the token, typically the User or Client Id. Must be an id of an existing User or " +
                   "Client in the database"
    )]
    [Range(0, int.MaxValue)]
    public int Subject { get; set; }

    [Option(
        "subject-type",
        Required = false,
        HelpText = $"Type of subject the token is being generated for." +
                   $"\nPossible values: ['{OtrClaims.Roles.User}', '{OtrClaims.Roles.Client}']",
        Default = OtrClaims.Roles.User
    )]
    public string SubjectType { get; set; } = OtrClaims.Roles.User;

    [Option(
        "token-type",
        Required = false,
        HelpText = $"Type of token being generated." +
                   $"\nPossible values: ['{OtrClaims.TokenTypes.AccessToken}', '{OtrClaims.TokenTypes.RefreshToken}']",
        Default = OtrClaims.TokenTypes.AccessToken
    )]
    public string TokenType { get; set; } = OtrClaims.TokenTypes.AccessToken;

    [Option(
        "roles",
        Required = false,
        HelpText = $"Any number of role claims granted to the subject." +
                   $"\nPossible values: ['{OtrClaims.Roles.Admin}', '{OtrClaims.Roles.Submitter}'," +
                   $"'{OtrClaims.Roles.Verifier}', '{OtrClaims.Roles.Whitelist}']"
    )]
    public IEnumerable<string> Roles { get; set; } = new List<string>();

    [Option(
        "rlo-permit-limit",
        Required = false,
        HelpText = "Sets a custom permit limit for the subject that overrides the API rate limit."
    )]
    public int? PermitLimit { get; set; }

    [Option(
        "rlo-window",
        Required = false,
        HelpText = "Sets a custom window interval (in seconds) for the subject that overrides the API rate limit."
    )]
    public int? Window { get; set; }

    [Option(
        "expiry",
        Required = false,
        HelpText = "The lifetime of the token (in seconds)." +
                   "\nDefault: 3600 (1 hour) for access tokens, 1_209_600 (2 weeks) for refresh tokens"
    )]
    public int? ExpiresIn { get; set; }

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
