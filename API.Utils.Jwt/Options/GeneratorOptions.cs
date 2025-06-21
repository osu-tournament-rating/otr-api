using System.ComponentModel.DataAnnotations;
using API.Authorization;
using CommandLine;
using JetBrains.Annotations;

namespace API.Utils.Jwt.Options;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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
        HelpText = "The lifetime of the token (in seconds)",
        Default = 3600
    )]
    public int ExpiresIn { get; set; } = 3600;
}
