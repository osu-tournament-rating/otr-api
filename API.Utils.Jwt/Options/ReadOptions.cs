using CommandLine;

namespace API.Utils.Jwt.Options;

[Verb("read", aliases: ["r"], HelpText = "Read an encoded JWT")]
public class ReadOptions : IJwtUtilsOptions
{
    public bool IsValid { get; set; }
}
