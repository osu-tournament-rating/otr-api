using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json;
using System.Threading.RateLimiting;
using API.Authorization;
using API.Authorization.Handlers;
using API.Authorization.Requirements;
using API.Configurations;
using API.Handlers.Implementations;
using API.Handlers.Interfaces;
using API.Middlewares;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.SwaggerGen;
using API.SwaggerGen.Filters;
using API.Utilities;
using API.Utilities.AdminNotes;
using API.Utilities.Extensions;
using Asp.Versioning;
using AutoMapper;
using Dapper;
using Database;
using Database.Entities;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Serialization;
using Npgsql;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OsuApiClient.Extensions;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using IMatchRosterRepository = Database.Repositories.Interfaces.IMatchRosterRepository;

#region WebApplicationBuilder Configuration

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#endregion

#region Configuration Bindings

builder
    .Services.AddOptionsWithValidateOnStart<ConnectionStringsConfiguration>()
    .Bind(builder.Configuration.GetSection(ConnectionStringsConfiguration.Position))
    .ValidateDataAnnotations();

builder
    .Services.AddOptionsWithValidateOnStart<OsuConfiguration>()
    .Bind(builder.Configuration.GetSection(OsuConfiguration.Position))
    .ValidateDataAnnotations();

builder
    .Services.AddOptionsWithValidateOnStart<JwtConfiguration>()
    .Bind(builder.Configuration.GetSection(JwtConfiguration.Position))
    .ValidateDataAnnotations();

builder
    .Services.AddOptionsWithValidateOnStart<AuthConfiguration>()
    .Bind(builder.Configuration.GetSection(AuthConfiguration.Position))
    .ValidateDataAnnotations();

builder
    .Services.AddOptionsWithValidateOnStart<RateLimitConfiguration>()
    .Bind(builder.Configuration.GetSection(RateLimitConfiguration.Position))
    .ValidateDataAnnotations();

#endregion

#region Controller Configuration

builder.Services
    .AddControllers(o =>
    {
        o.ModelMetadataDetailsProviders.Add(
            new NewtonsoftJsonValidationMetadataProvider(new CamelCaseNamingStrategy()));
        o.Filters.Add(new AuthorizeFilter(AuthorizationPolicies.Whitelist));
    })
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddNewtonsoftJson(o => { o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });

#endregion

#region OpenTelemetry Tracing/Metrics Configuration

builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
{
    options.EnrichWithException = (activity, exception) =>
    {
        activity.SetTag("stackTrace", exception.StackTrace);
        activity.SetTag("errorMessage", exception.Message);
        activity.SetTag("innerException", exception.InnerException); //might not need
        activity.SetTag("exceptionSource", exception.Source);
    };
});

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService(serviceName: builder.Environment.ApplicationName)
    )
    .WithTracing(tracing =>
        tracing
            .AddAspNetCoreInstrumentation()
            .AddNpgsql()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(
                    builder
                        .Configuration.BindAndValidate<ConnectionStringsConfiguration>(
                            ConnectionStringsConfiguration.Position
                        )
                        .CollectorConnection
                );
            })
    )
    .WithMetrics(b =>
        b.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter(o => o.DisableTotalNameSuffixForCounters = true)
    );

#endregion

#region Rate Limit Configuration

builder.Services.AddRateLimiter(options =>
{
    RateLimitConfiguration rateLimitConfiguration =
        builder.Configuration.BindAndValidate<RateLimitConfiguration>(
            RateLimitConfiguration.Position
        );

    // Configure response for rate limited clients
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        const string readMore =
            // TODO: Link to the API Terms of Service Document
            "Currently, detailed information about API rate limits is unavailable.";
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            await context.HttpContext.Response.WriteAsync(
                $"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s). "
                + readMore,
                token
            );
        }
        else
        {
            await context.HttpContext.Response.WriteAsync(
                "Too many requests. Please try again later. " + readMore,
                token
            );
        }
    };

    // Configure the rate limit partitioning rules
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        // Shared partition for anonymous requests
        if (context.User.Identity is { IsAuthenticated: false })
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                "anonymous",
                _ => GetRateLimiterOptions()
            );
        }

        // Partition for each unique user / client
        return RateLimitPartition.GetFixedWindowLimiter(
            $"{context.User.GetSubjectType()}_{context.User.GetSubjectId()}",
            _ => GetRateLimiterOptions(context.User.GetRateLimitOverride())
        );
    });

    return;

    FixedWindowRateLimiterOptions GetRateLimiterOptions(int? limitOverride = null) => new()
    {
        PermitLimit = limitOverride ?? rateLimitConfiguration.PermitLimit,
        Window = TimeSpan.FromSeconds(rateLimitConfiguration.Window),
        QueueLimit = 0
    };
});

#endregion

#region SwaggerGen Configuration

builder.Services
    .AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        // ReSharper disable once StringLiteralTypo
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddSwaggerGen(options =>
{
    // Sets nullable flags
    options.SupportNonNullableReferenceTypes();
    // Allows reference enums to be marked nullable
    options.UseAllOfToExtendReferenceSchemas();
    // Allows reference objects to be marked nullable
    options.UseAllOfForInheritance();
    // Fixes parameter names from model binding
    options.DescribeAllParametersInCamelCase();

    // Allow use of in-code XML documentation tags like <summary> and <remarks>
    options.IncludeXmlCommentsWithRemarks($"{AppDomain.CurrentDomain.BaseDirectory}API.xml");

    // Register custom filters.
    // Filters are executed in order of: Operation, Parameter, Schema, Document
    options.OperationFilter<SecurityMetadataOperationFilter>();
    options.OperationFilter<DiscardNestedParametersOperationFilter>();

    options.SchemaFilter<OverrideSchemaFilter<AdminNoteRouteTarget>>((OpenApiSchema schema, SchemaFilterContext _) =>
    {
        // Only target the schema definition, not references
        if (schema.AllOf.Any())
        {
            return;
        }

        schema.Type = "string";
        schema.Enum = AdminNotesHelper.GetAdminNoteableEntityRoutes().ToOpenApiArray();
        schema.Extensions = new Dictionary<string, IOpenApiExtension>
        {
            [ExtensionKeys.EnumNames] = AdminNotesHelper.GetAdminNoteableEntityTypes()
                .Select(t => t.Name)
                .ToOpenApiArray()
        };
    });

    options.SchemaFilter<EnumMetadataSchemaFilter>();
    options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();

    // Populate the document's info
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "osu! Tournament Rating API",
            Description =
                "The official resource for reading and writing data within the osu! Tournament Rating platform."
        }
    );

    // Generate custom descriptors for endpoints using their literal method names
    var unknownMethodCount = 0;
    options.CustomOperationIds(description =>
    {
        var controller = description.ActionDescriptor.RouteValues["controller"] ?? "undocumented";
        string method;

        if (description.TryGetMethodInfo(out MethodInfo info))
        {
            var methodName = info.Name.Replace("Async", string.Empty, StringComparison.OrdinalIgnoreCase);
            method = char.ToLower(methodName[0]) + methodName[1..];
        }
        else
        {
            method = $"method_{unknownMethodCount++}";
        }

        return $"{controller}_{method}";
    });

    // Add documentation about authentication schemes
    var oauthScopes = OtrClaims.Roles.ValidRoles
        .Select(r => new KeyValuePair<string, string>(r, OtrClaims.GetDescription(r)))
        .ToDictionary();

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "JWT Authentication",
            Type = SecuritySchemeType.Http,
            Description =
                "JWT Authorization using the Bearer scheme. Paste **ONLY** your JWT in the text box below",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT"
        });

    options.AddSecurityDefinition(SecurityRequirements.OAuthSecurityRequirementId,
        new OpenApiSecurityScheme
        {
            Name = "OAuth2 Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.OAuth2,
            Description = "OAuth2 Authentication",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("api/v1.0/OAuth/authorize", UriKind.Relative),
                    RefreshUrl = new Uri("api/v1.0/OAuth/refresh", UriKind.Relative),
                    Scopes = oauthScopes
                },
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("api/v1.0/OAuth/token", UriKind.Relative),
                    RefreshUrl = new Uri("api/v1.0/OAuth/refresh", UriKind.Relative),
                    Scopes = oauthScopes
                }
            }
        });

    // Register custom enum schemas describing authorization roles and policies
    options.DocumentFilter<RegisterCustomSchemaDocumentFilter>(nameof(OtrClaims.Roles),
        new OpenApiSchema
        {
            Type = "string",
            Description = "The possible roles assignable to a user or client",
            Enum = [.. oauthScopes.Keys.Select(role => new OpenApiString(role))],
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                [ExtensionKeys.EnumNames] =
                    oauthScopes.Keys.Select(k => char.ToUpper(k[0]) + k[1..]).ToOpenApiArray(),
                [ExtensionKeys.EnumDescriptions] = oauthScopes.Values.ToOpenApiArray()
            }
        });

    // Register custom enum schemas describing authorization roles and policies
    options.DocumentFilter<RegisterCustomSchemaDocumentFilter>(nameof(AuthorizationPolicies), new OpenApiSchema
    {
        Type = "string",
        Description = "The possible authorization policies enforced on a route. Authorization policies differ from " +
                      "Roles as they may require special conditions to be satisfied. See the description of a " +
                      "policy for more information.",
        Enum = AuthorizationPolicies.Policies.ToOpenApiArray(),
        Extensions = new Dictionary<string, IOpenApiExtension>
        {
            [ExtensionKeys.EnumNames] = AuthorizationPolicies.Policies.ToOpenApiArray(),
            [ExtensionKeys.EnumDescriptions] = AuthorizationPolicies.Policies
                .Select(AuthorizationPolicies.GetDescription)
                .ToOpenApiArray()
        }
    });

    // Add the ability to authenticate with swagger ui
    options.AddSecurityRequirement(SecurityRequirements.BearerSecurityRequirement);
});

#endregion

#region Logging Configuration

builder.Services.AddSerilog(configuration =>
{
    var connString = builder
        .Configuration.BindAndValidate<ConnectionStringsConfiguration>(
            ConnectionStringsConfiguration.Position
        )
        .DefaultConnection;

#if DEBUG
    configuration.MinimumLevel.Debug();
#else
    configuration.MinimumLevel.Information();
#endif

    configuration
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override(
            "Microsoft.EntityFrameworkCore.Database.Command",
            LogEventLevel.Warning
        )
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.File(
            Path.Join("logs", "log.log"),
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information
        )
        .WriteTo.PostgreSQL(
            connString,
            "Logs",
            needAutoCreateTable: true,
            restrictedToMinimumLevel: LogEventLevel.Warning
        );
});

builder.Services.Configure<RequestLoggingOptions>(o =>
{
    o.IncludeQueryInRequestPath = true;

    o.MessageTemplate =
        "{RequestIdentity} {RequestScheme} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.00} ms";

    o.GetLevel = (httpContext, elapsed, ex) =>
    {
        // Error for server errors and exceptions
        if (httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError || ex is not null)
        {
            return LogEventLevel.Error;
        }

        // Warn for unexpected client errors or long response times
        if (httpContext.Response.StatusCode > StatusCodes.Status404NotFound || elapsed >= 5000)
        {
            return LogEventLevel.Warning;
        }

        return LogEventLevel.Information;
    };

    o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);

        string ident;
        if (httpContext.User.TryGetSubjectId(out var id))
        {
            var subjectType = httpContext.User.GetSubjectType();
            subjectType = char.ToUpper(subjectType[0]) + subjectType[1..];
            ident = $"{subjectType} {id}";
        }
        else
        {
            ident = "Anonymous";
        }

        diagnosticContext.Set("RequestIdentity", ident);
    };
});

#endregion

#region CORS Configuration

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .WithOrigins("https://staging.otr.stagec.xyz", "https://otr.stagec.xyz")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

#endregion

#region Authentication Configuration

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JsonWebTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        JwtConfiguration jwtConfiguration = builder.Configuration.BindAndValidate<JwtConfiguration>(
            JwtConfiguration.Position
        );

        options.MapInboundClaims = false;

        options.TokenValidationParameters = DefaultTokenValidationParameters.Get(
            jwtConfiguration.Issuer,
            jwtConfiguration.Key,
            jwtConfiguration.Audience
        );

        options.Events = new JwtBearerEvents
        {
            // Reject authentication challenges not using an access token (or that don't contain a token type)
            OnTokenValidated = context =>
            {
                if (context.Principal?.GetTokenType() is not OtrClaims.TokenTypes.AccessToken)
                {
                    context.Fail("Invalid token type. Only access tokens are accepted.");
                }

                return Task.CompletedTask;
            }
        };
    });

#endregion

#region Authorization Configuration

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.AccessUserResources, p =>
    {
        p.RequireRole(OtrClaims.Roles.User);
        p.AddRequirements(new AccessUserResourcesAuthorizationRequirement());
    })
    .AddPolicy(AuthorizationPolicies.Whitelist, p => { p.AddRequirements(new WhitelistAuthorizationRequirement()); });

builder.Services.AddScoped<IAuthorizationHandler, AccessUserResourcesAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, WhitelistAuthorizationHandler>();

#endregion

#region Context Accessors

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddHttpContextAccessor();

#endregion

#region Dapper

DefaultTypeMap.MatchNamesWithUnderscores = true;

#endregion

#region AutoMapper

builder.Services.AddAutoMapper(typeof(Program).Assembly);

#endregion

#region Database Context

builder.Services.AddScoped<AuditBlamingInterceptor>();
builder.Services.AddDbContext<OtrContext>((services, options) =>
{
    options
        .UseNpgsql(builder.Configuration.BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).DefaultConnection)
        .AddInterceptors(services.GetRequiredService<AuditBlamingInterceptor>())
        .UseSnakeCaseNamingConvention();
});

// The Redis cache is registered as a singleton because it is meant to be re-used across instances
builder.Services.AddSingleton<ICacheHandler>(
    new CacheHandler(
        builder
            .Configuration.BindAndValidate<ConnectionStringsConfiguration>(
                ConnectionStringsConfiguration.Position
            )
            .RedisConnection
    )
);

builder.Services.AddScoped<IPasswordHasher<OAuthClient>, PasswordHasher<OAuthClient>>();

#endregion

#region Handlers

builder.Services.AddScoped<IOAuthHandler, OAuthHandler>();

#endregion

#region Repositories

builder.Services.AddScoped<IAdminNoteRepository, AdminNoteRepository>();
builder.Services.AddScoped<IBeatmapsRepository, BeatmapsRepository>();
builder.Services.AddScoped<IGamesRepository, GamesRepository>();
builder.Services.AddScoped<IGameScoresRepository, GameScoresRepository>();
builder.Services.AddScoped<IGameWinRecordsRepository, GameWinRecordsRepository>();
builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
builder.Services.AddScoped<IRatingAdjustmentsRepository, RatingAdjustmentsRepository>();
builder.Services.AddScoped<IMatchRosterRepository, MatchRosterRepository>();
builder.Services.AddScoped<IOAuthClientRepository, OAuthClientRepository>();
builder.Services.AddScoped<IPlayerMatchStatsRepository, PlayerMatchStatsRepository>();
builder.Services.AddScoped<IPlayerRatingsRepository, PlayerRatingsRepository>();
builder.Services.AddScoped<IPlayerTournamentStatsRepository, PlayerTournamentStatsRepository>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

#endregion

#region Services

builder.Services.AddScoped<IAdminNoteService, AdminNoteService>();
builder.Services.AddScoped<IBeatmapService, BeatmapService>();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IGameScoresService, GameScoresService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IOAuthClientService, OAuthClientService>();
builder.Services.AddScoped<IPlayerRatingsService, PlayerRatingsService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IFilteringService, FilteringService>();
builder.Services.AddScoped<ITournamentsService, TournamentsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUrlHelperService, UrlHelperService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();

#endregion

#region osu! Api

builder.Services.AddOsuApiClient(builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position));

#endregion

#region WebApplication Configuration

WebApplication app = builder.Build();

// Set switch for Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

// UseRouting must come first before UseCors
app.UseRouting();
// Placed after UseRouting and before UseAuthentication and UseAuthorization
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.UseMiddleware<RateLimitHeadersMiddleware>();

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    // only during development, validate your mappings
    IMapper mapper = app.Services.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();

    app.UseSwaggerUI(o =>
    {
        o.EnableTryItOutByDefault();
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "osu! Tournament Rating API");
    });

    if (args.Contains("--allow-anonymous"))
    {
        // Endpoints / features expecting authentication (like `GET` `/me`) WILL throw exceptions if used anonymously
        app.MapControllers().AllowAnonymous();
    }
    else
    {
        app.MapControllers();
    }
}
else
{
    app.MapControllers();
}

#endregion

#region Swagger Doc Generation

if (args.Contains("--swagger-to-file"))
{
    app.Logger.LogInformation("Saving Swagger spec to file");

    // Get the swagger document provider from the service container
    OpenApiDocument? swagger = app.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");
    if (swagger is null)
    {
        app.Logger.LogError("Could not resolve the swagger spec, exiting");
        return;
    }

    // Serialize the swagger doc to JSON
    var stringWriter = new StringWriter();
    swagger.SerializeAsV3(new OpenApiJsonWriter(stringWriter));

    // Write to base path
    var path = $"{AppDomain.CurrentDomain.BaseDirectory}swagger.json";
    File.WriteAllText(path, stringWriter.ToString());

    app.Logger.LogInformation("Swagger spec saved to: {Path}", path);

    return;
}

#endregion

app.Logger.LogInformation("Running!");

app.Run();
