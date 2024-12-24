using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using API.Authorization;
using CommandLine;

namespace API.Utils.Jwt.Options;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[Verb("generate", isDefault: true, HelpText = "Generate a new JWT")]
public class GeneratorOptions : JwtUtilsOptionsBase
{
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
                   $"'{OtrClaims.Roles.Verifier}', '{OtrClaims.Roles.Whitelist}']",
        Separator = ','
    )]
    public IEnumerable<string> Roles { get; set; } = [];

    [Option(
        "rate-limit-override",
        Required = false,
        HelpText = "Sets a custom permit limit for the subject that overrides the API rate limit."
    )]
    public int? PermitLimit { get; set; }

    [Option(
        "expiry",
        Required = false,
        HelpText = "The lifetime of the token (in seconds)." +
                   "\nDefault: 3600 (1 hour) for access tokens, 1_209_600 (2 weeks) for refresh tokens"
    )]
    public int? ExpiresIn { get; set; }
}
