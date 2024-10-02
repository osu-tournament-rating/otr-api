using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Authorization;
using API.Utils.Jwt.Options;
using CommandLine;
using Database.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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

        Parser.Default.ParseArguments<GeneratorOptions, ReadOptions>(args)
            .WithParsed<GeneratorOptions>(Generate)
            .WithParsed<ReadOptions>(Read)
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
            )
        }));

        Log.Information("");
        Log.Information("Token Created");
        Log.Information("----------------------------------------");
        Log.Information(token);
        Log.Information("----------------------------------------");
    }

    private static void Read(ReadOptions o)
    {
        Log.Information("Validating options...");
        o.PostConfigure();

        if (!o.IsValid)
        {
            Log.Error("Could not validate given options, exiting...");
            return;
        }

        Log.Information("Reading token...");

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = tokenHandler.ReadJwtToken(o.Token);

        Log.Information("");
        Log.Information("Header: '{Header}'", token.RawHeader);
        Log.Information("-------------------------------------------------------");
        Log.Information("\n" + JsonConvert.SerializeObject(token.Header, Formatting.Indented));
        Log.Information("");
        Log.Information("Payload: '{Payload}'", token.RawPayload);
        Log.Information("-------------------------------------------------------");
        Log.Information("\n" + JsonConvert.SerializeObject(token.Payload, Formatting.Indented));
        Log.Information("");
        Log.Information("Sig: '{Sig}'", token.RawSignature);

        if (!o.Validate)
        {
            return;
        }

        Log.Information("");
        Log.Information("Attempting to validate token...");

        ClaimsPrincipal? principal = null;
        try
        {
            principal = tokenHandler.ValidateToken(
                o.Token,
                DefaultTokenValidationParameters.Get(o.Issuer, o.Key, o.Audience),
                out SecurityToken _
            );
        }
        catch (Exception ex)
        {
            Log.Error("Token could not be validated!");
            Log.Error(ex.Message);
        }

        if (principal?.Identity is { IsAuthenticated: true })
        {
            Log.Information("Token is valid!");
            return;
        }

        Log.Error("Token could not be validated!");
    }

    private static void HandleParseErrors(IEnumerable<Error> errors)
    {

    }
}
