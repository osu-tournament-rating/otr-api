﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Authorization;
using API.Utils.Jwt.Options;
using CommandLine;
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

        Parser.Default.ParseArguments<GeneratorOptions, ReadOptions>(args)
            .WithParsed<GeneratorOptions>(Generate)
            .WithParsed<ReadOptions>(Read);
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
            new(OtrClaims.Subject, o.Subject.ToString()),
            new(OtrClaims.Role, o.SubjectType),
            new(OtrClaims.Instance, Guid.NewGuid().ToString())
        };
        claims.AddRange(o.Roles.Select(r => new Claim(OtrClaims.Role, r)));

        if (o.PermitLimit.HasValue)
        {
            claims.Add(new Claim(OtrClaims.RateLimitOverrides, o.PermitLimit.Value.ToString()));
        }

        Log.Information("Writing a JWT with options:\n{Opts}", o.ToString());

        var tokenHandler = new JwtSecurityTokenHandler();
        string token = tokenHandler.WriteToken(tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(o.ExpiresIn),
            Audience = o.Audience,
            Issuer = o.Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(o.Key)),
                SecurityAlgorithms.HmacSha256
            )
        }));

        Log.Information("Token Created");
        Log.Information("----------------------------------------");
        Log.Information("{Token}", token);
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
        Log.Information("Header:");
        Log.Information("{@Header}", token.Header);
        Log.Information("");
        Log.Information("Payload:");
        Log.Information("{@Payload}", token.Payload);
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
            Log.Error("Token could not be validated! {Exception}", ex.Message);
        }

        if (principal?.Identity is { IsAuthenticated: true })
        {
            Log.Information("Token is valid!");
            return;
        }

        Log.Error("Token is not valid!");
    }
}
