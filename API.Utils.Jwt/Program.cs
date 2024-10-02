using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Authorization;
using API.Utils.Jwt.Options;
using CommandLine;
using Database.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace API.Utils.Jwt;

public static class Program
{
    public static void Main(string[] args)
    {
        # region Static Config

        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger();
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JsonWebTokenHandler.DefaultMapInboundClaims = false;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        # endregion

        Parser.Default.ParseArguments<GeneratorOptions, ReadOptions, WriteOptions>(args)
            .WithParsed<GeneratorOptions>(Generate)
            .WithParsed<ReadOptions>(Read)
            .WithParsed<WriteOptions>(Write)
            .WithNotParsed(HandleParseErrors);
    }

    private static void Generate(GeneratorOptions o)
    {
        Log.Information("Validating options...");
        o.PostConfigure();

        if (!o.IsValid)
        {
            Log.Error("Could not validate given options, exiting...");
            return;
        }

        // Build claims
        var claims = new List<Claim>
        {
            new(OtrClaims.TokenType, o.TokenType),
            new(OtrClaims.Subject, o.Subject.ToString()),
            new(OtrClaims.Role, o.SubjectType),
            new(OtrClaims.Instance, Guid.NewGuid().ToString())
        };
        claims.AddRange(o.Roles.Select(r => new Claim(OtrClaims.Role, r)));

        if (o.PermitLimit.HasValue || o.Window.HasValue)
        {
            claims.Add(new Claim(
                OtrClaims.RateLimitOverrides,
                RateLimitOverridesSerializer.Serialize(new RateLimitOverrides
                {
                    PermitLimit = o.PermitLimit,
                    Window = o.Window
                }
                )
            ));
        }

        Log.Information("Writing a JWT with options:\n{Opts}", o.ToString());

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(o.ExpiresIn!.Value),
            Audience = o.Audience,
            Issuer = o.Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key)),
                SecurityAlgorithms.HmacSha256
            ),
        }));

        Log.Information("");
        Log.Information("Token Created");
        Log.Information("----------------------------------------");
        Log.Information(token);
        Log.Information("----------------------------------------");
    }

    private static void Read(ReadOptions o)
    {
        o.PostConfigure();
    }

    private static void Write(WriteOptions o)
    {
        o.PostConfigure();
    }

    private static void HandleParseErrors(IEnumerable<Error> errors)
    {

    }
}
