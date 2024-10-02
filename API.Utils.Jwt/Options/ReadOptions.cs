using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace API.Utils.Jwt.Options;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[Verb("read", HelpText = "Read an encoded JWT")]
public class ReadOptions : JwtUtilsOptionsBase
{
    [Option(
        't',
        "token",
        Required = true,
        HelpText = "The token to read."
    )]
    public string Token { get; set; } = null!;

    [Option(
        "validate",
        Required = false,
        HelpText = "Whether or not to validate the token (validates in the same manner as the API)." +
                   "\nRequires values from '--issuer', '--key', and '--audience' or supplied from a config file."
    )]
    public bool Validate { get; set; }
}
