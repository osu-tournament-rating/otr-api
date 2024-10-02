using CommandLine;

namespace API.Utils.Jwt.Options;

[Verb("write", aliases: ["w"], HelpText = "Write a JWT from JSON")]
public class WriteOptions : IJwtUtilsOptions
{
    public bool IsValid { get; set; }
}
