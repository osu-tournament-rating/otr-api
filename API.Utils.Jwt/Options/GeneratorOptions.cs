using System.Diagnostics.CodeAnalysis;
using API.Authorization;
using CommandLine;
using Database.Entities;

namespace API.Utils.Jwt.Options;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
[Verb("generate", isDefault: true, aliases: ["gen"], HelpText = "Generate a new JWT")]
public class GeneratorOptions
{
    [Option(
        "subject",
        Required = true,
        HelpText = "Subject of the token, typically the User or Client Id. Must be an id of an existing User or " +
                   "Client in the database"
    )]
    public int Subject { get; set; }

    [Option(
        "subject-type",
        Required = false,
        HelpText = $"Type of subject the token is being generated for, must be either " +
                   $"'{OtrClaims.Roles.User}' or '{OtrClaims.Roles.Client}'",
        Default = OtrClaims.Roles.User
    )]
    public string SubjectType { get; set; } = OtrClaims.Roles.User;

    [Option(
        "token-type",
        Required = false,
        HelpText = $"Type of token being generated, must be either " +
                   $"'{OtrClaims.TokenTypes.AccessToken}' or '{OtrClaims.TokenTypes.RefreshToken}'",
        Default = OtrClaims.TokenTypes.AccessToken
    )]
    public string TokenType { get; set; } = OtrClaims.TokenTypes.AccessToken;

    [Option(
        "expiry",
        Required = false,
        HelpText = "The lifetime of the token in seconds. " +
                   "Default: 3600 (1 hour) for access tokens, 1_209_600 (2 weeks) for refresh tokens"
    )]
    public int ExpiresIn { get; set; }

    [Option(
        "roles",
        Required = false,
        HelpText = $"Any number of role claims granted to the subject. " +
                   $"Must be valid roles from {nameof(OtrClaims.Roles)}"
    )]
    public string[] Roles { get; set; } = [];

    [Option(
        "rlo-permit-limit",
        Required = false,
        HelpText = $"Sets the {nameof(RateLimitOverrides)}.{nameof(RateLimitOverrides.PermitLimit)} for the subject."
    )]
    public int? PermitLimit { get; set; }

    [Option(
        "rlo-window",
        Required = false,
        HelpText = $"Sets the {nameof(RateLimitOverrides)}.{nameof(RateLimitOverrides.Window)} for the subject."
    )]
    public int? Window { get; set; }

    [Option(
        'i',
        "issuer",
        Required = false,
        HelpText = "Issuer of the token, typically the domain of the API. " +
                   "Will attempt to populate from API/appsettings.Development.json if not given. " +
                   "Example: 'localhost:5075'"
    )]
    public string Issuer { get; set; } = string.Empty;

    [Option(
        'k',
        "key",
        Required = false,
        HelpText = "Signing key for the token. " +
                   "Will attempt to populate from API/appsettings.Development.json if not given."
    )]
    public string Key { get; set; } = string.Empty;

    [Option(
        'a',
        "audience",
        Required = false,
        HelpText = "Audience of the token, typically the domain of the website. " +
                   "Will attempt to populate from API/appsettings.Development.json if not given." +
                   "Example: 'localhost:3000'"
    )]
    public string Audience { get; set; } = string.Empty;

    [Option(
        "config",
        Required = false,
        HelpText = "Path to an appsettings.json file containing a JWT configuration. " +
                   "JWT configuration supplied from the command line will take priority " +
                   "Example: '~/github/otr-api/API/appsettings.Development.json'"
    )]
    public string ConfigFile { get; set; } = string.Empty;

    [Option(
        'o',
        "out-file",
        Required = false,
        HelpText = "Path to a file for writing output. If not set, output will only print in console"
    )]
    public string? OutFile { get; set; }
}
