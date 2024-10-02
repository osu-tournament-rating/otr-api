using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using API.Authorization;
using API.Configurations;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace API.Utils.Jwt.Options;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class OptionsExtensions
{
    /// <summary>
    /// Performs input validation on an instance of <see cref="GeneratorOptions"/>.
    /// Will log errors and set <see cref="GeneratorOptions.IsValid"/> based on the result
    /// </summary>
    public static void PostConfigure(this GeneratorOptions o)
    {
        // Might not be necessary because of data annotation
        if (o.Subject <= 0)
        {
            LogConfigureError(o.GetArgLongName(nameof(o.Subject)), o.Subject);
            return;
        }

        // Validate subject type
        if (o.SubjectType is not OtrClaims.Roles.User && o.SubjectType is not OtrClaims.Roles.Client)
        {
            LogConfigureError(
                o.GetArgLongName(nameof(o.SubjectType)),
                o.SubjectType,
                $"Must be one of: '{OtrClaims.Roles.User}' or '{OtrClaims.Roles.Client}'"
            );
            return;
        }

        // Validate token type
        if (o.TokenType is not OtrClaims.TokenTypes.AccessToken
            && o.TokenType is not OtrClaims.TokenTypes.RefreshToken)
        {
            LogConfigureError(
                o.GetArgLongName(nameof(o.TokenType)),
                o.TokenType,
                $"Must be one of: '{OtrClaims.TokenTypes.AccessToken}' or '{OtrClaims.TokenTypes.RefreshToken}'"
            );
            return;
        }

        // Validate roles
        foreach (var role in o.Roles)
        {
            if (!OtrClaims.Roles.IsValidRole(role))
            {
                Log.Error(
                    "Invalid role: '{Role}'\nPossible values: [{PossibleValues}]",
                    role,
                    OtrClaims.Roles.ValidRoles
                );
                return;
            }

            switch (o.SubjectType)
            {
                case OtrClaims.Roles.User when !OtrClaims.Roles.IsUserAssignableRole(role):
                    Log.Error(
                        "Invalid '{Prop}': '{Val}'\nRole is not assignable to '{SubjectTypeProp}' of '{SubjectType}'" +
                        "\nPossible values: [{PossibleValues}]",
                        o.GetArgLongName(nameof(o.Roles)),
                        role,
                        o.GetArgLongName(nameof(o.SubjectType)),
                        o.SubjectType,
                        OtrClaims.Roles.UserAssignableRoles
                    );
                    return;
                case OtrClaims.Roles.Client when !OtrClaims.Roles.IsClientAssignableRole(role):
                    Log.Error(
                        "Invalid '{Prop}': '{Val}'\nRole is not assignable to '{SubjectTypeProp}' of '{SubjectType}'" +
                        "\nPossible values: [{PossibleValues}]",
                        o.GetArgLongName(nameof(o.Roles)),
                        role,
                        o.GetArgLongName(nameof(o.SubjectType)),
                        o.SubjectType,
                        OtrClaims.Roles.ClientAssignableRoles
                    );
                    return;
            }
        }

        // Validate rate limit overrides
        if (o.PermitLimit is <= 0)
        {
            LogConfigureError(o.GetArgLongName(nameof(o.PermitLimit)), o.PermitLimit);
            return;
        }

        if (o.Window is <= 0)
        {
            LogConfigureError(o.GetArgLongName(nameof(o.Window)), o.Window);
            return;
        }

        // Validate expiry
        o.ExpiresIn ??= o.TokenType switch
        {
            OtrClaims.TokenTypes.RefreshToken => 1_209_600,
            _ => 3600
        };
        if (o.ExpiresIn <= 0)
        {
            LogConfigureError(o.GetArgLongName(nameof(o.ExpiresIn)), o.ExpiresIn);
            return;
        }

        if (!o.ValidateJwtConfig())
        {
            return;
        }

        // All validations passed, configuration can be used to generate an o!TR compliant JWT
        o.IsValid = true;
    }

    /// <summary>
    /// Performs input validation on an instance of <see cref="ReadOptions"/>.
    /// Will log errors and set <see cref="ReadOptions.IsValid"/> based on the result
    /// </summary>
    public static void PostConfigure(this ReadOptions o)
    {
        if (o.Validate)
        {
            if (!o.ValidateJwtConfig())
            {
                return;
            }
        }

        o.IsValid = true;
    }

    /// <summary>
    /// Validates the config-file based properties of an instance of <see cref="JwtUtilsOptionsBase"/>
    /// </summary>
    /// <returns>True if config is valid, false if not</returns>
    private static bool ValidateJwtConfig(this JwtUtilsOptionsBase o)
    {
        // Try to load JWT configuration values from file
        var jwtCfgFromFile = new JwtConfiguration();
        if (!string.IsNullOrEmpty(o.ConfigFile))
        {
            try
            {
                new ConfigurationBuilder()
                    .AddJsonFile(Path.GetFullPath(o.ConfigFile))
                    .Build()
                    .GetSection(JwtConfiguration.Position)
                    .Bind(jwtCfgFromFile);
            }
            catch (Exception ex)
            {
                Log.Error(
                    "Error loading configuration file: '{ConfigFile}'\n{Ex}",
                    o.ConfigFile,
                    ex
                );
                Log.Information("Attempting to continue validating params");
            }
        }

        // Create a final configuration to validate annotations letting command line values take priority
        var jwtCfg = new JwtConfiguration
        {
            Audience = string.IsNullOrEmpty(o.Audience) ? jwtCfgFromFile.Audience : o.Audience,
            Issuer = string.IsNullOrEmpty(o.Issuer) ? jwtCfgFromFile.Issuer : o.Issuer,
            Key = string.IsNullOrEmpty(o.Key) ? jwtCfgFromFile.Key : o.Key
        };

        // Validate data annotations
        try
        {
            jwtCfg.ValidateDataAnnotations();
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(
                "Could not validate values '{Aud}', '{Iss}', or '{Key}'\n{Ex}",
                o.GetArgLongName(nameof(o.Audience)),
                o.GetArgLongName(nameof(o.Issuer)),
                o.GetArgLongName(nameof(o.Key)),
                ex
            );
            return false;
        }

        // Copy validated values back to the config
        o.Audience = jwtCfg.Audience;
        o.Issuer = jwtCfg.Issuer;
        o.Key = jwtCfg.Key;

        return true;
    }

    /// <summary>
    /// Validates any <see cref="System.ComponentModel.DataAnnotations"/> attributes for an instance
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if any annotations fail validation</exception>
    private static void ValidateDataAnnotations<T>(this T config)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            config!,
            new ValidationContext(config!, serviceProvider: null, items: null),
            validationResults,
            true
        );

        if (isValid)
        {
            return;
        }

        var errorMessages = validationResults.Select(result => result.ErrorMessage).ToArray();
        throw new InvalidOperationException(
            $"Configuration validation failed for {nameof(T)}: {string.Join(", ", errorMessages)}"
        );
    }

    /// <summary>
    /// Gets the <see cref="OptionAttribute.LongName"/> of an <see cref="JwtUtilsOptionsBase"/> property
    /// </summary>
    private static string GetArgLongName(this JwtUtilsOptionsBase options, string propName) =>
        options.GetType().GetProperty(propName)?.GetCustomAttribute<OptionAttribute>()?.LongName ?? string.Empty;

    /// <summary>
    /// Logs an input validation error with a pre-set template
    /// </summary>
    /// <param name="prop">Long name of the command line option, see <see cref="GetArgLongName"/></param>
    /// <param name="value">Value of the property</param>
    /// <param name="hint">Optional hint text</param>
    private static void LogConfigureError(string prop, object value, string? hint = null)
    {
        if (string.IsNullOrEmpty(hint))
        {
            Log.Error(
                "Invalid '{Prop}': '{Val}'" + "\nUse 'jwtutil --help' for more information",
                prop,
                value
            );
        }

        Log.Error(
            "Invalid '{Prop}': '{Val}'" + "\nHint: {Hint}" + "\nUse 'jwtutil --help' for more information",
            prop,
            value,
            hint
        );
    }
}
