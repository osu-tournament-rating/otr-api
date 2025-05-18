using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
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
using OsuApiClient;
using OsuApiClient.Domain.Osu.Users;
using OsuApiClient.Extensions;
using OsuApiClient.Net.Authorization;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using IMatchRosterRepository = Database.Repositories.Interfaces.IMatchRosterRepository;
using User = Database.Entities.User;

#pragma warning disable SYSLIB1045
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

builder.Services.AddOutputCache();
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

builder.Services.AddRouting(options => options.LowercaseUrls = true);

#endregion

#region OpenTelemetry Tracing/Metrics Configuration

const string serviceName = "otr-api";

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
});

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
        resource.AddService(serviceName: serviceName)
    )
    .WithTracing(tracing =>
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Filter = httpContext =>
                {
                    try
                    {
                        // Tracing does not need to be flooded with these requests from prometheus
                        return httpContext.Request.Path.Value != "/metrics";
                    }
                    catch
                    {
                        return false;
                    }
                };
            })
            .AddHttpClientInstrumentation()
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
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Routing")
            .AddMeter("Microsoft.AspNetCore.Diagnostics")
            .AddMeter("Microsoft.EntityFrameworkCore")
            .AddProcessInstrumentation()
            .AddRuntimeInstrumentation()
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
        // Unlimited partition for anonymous requests, as doing so
        // would cripple the ability for the platform to support
        // an influx of legitimate anonymous requests
        if (context.User.Identity is { IsAuthenticated: false })
        {
            return RateLimitPartition.GetNoLimiter("anonymous");
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
                "The official resource for reading and writing data within the osu! Tournament Rating platform.",
            TermsOfService = new Uri("https://docs.otr.stagec.xyz/About/Terms-of-Use"),
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
    var authRoles = OtrClaims.Roles.ValidRoles
        .Select(r => new KeyValuePair<string, string>(r, OtrClaims.GetDescription(r)))
        .ToDictionary();

    // Register custom enum schemas describing authorization roles and policies
    options.DocumentFilter<RegisterCustomSchemaDocumentFilter>(nameof(OtrClaims.Roles),
        new OpenApiSchema
        {
            Type = "string",
            Description = "The possible roles assignable to a user or client",
            Enum = [.. authRoles.Keys.Select(role => new OpenApiString(role))],
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                [ExtensionKeys.EnumNames] =
                    authRoles.Keys.Select(k => char.ToUpper(k[0]) + k[1..]).ToOpenApiArray(),
                [ExtensionKeys.EnumDescriptions] = authRoles.Values.ToOpenApiArray()
            }
        });

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
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithSpan()
        .WriteTo.GrafanaLoki(builder.Configuration.BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).LokiConnection, new List<LokiLabel>
        {
            new() { Key = "app", Value = serviceName }
        }, ["app"])
        .WriteTo.Logger(lc => lc
            .Filter
            .ByExcluding(e => e.MessageTemplate.Text.Contains("Microsoft.EntityFrameworkCore.Database.Command"))
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [trace_id: {TraceId} span_id: {SpanId}] {Message:lj}{NewLine}"))
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
    options.AddDefaultPolicy(policy =>
    {
        AuthConfiguration authConfiguration = builder.Configuration.BindAndValidate<AuthConfiguration>(
            AuthConfiguration.Position
        );

        policy
            .WithOrigins(authConfiguration.AllowedHosts)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#endregion

#region Authentication Configuration

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
JsonWebTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication("CompositeAuthentication")
    .AddPolicyScheme("CompositeAuthentication", "CompositeAuthentication", options =>
    {
        // Use cookie-based authentication if possible and fallback to JWT Bearer
        options.ForwardDefaultSelector = context => context.Request.Cookies.ContainsKey("otr-session")
            ? CookieAuthenticationDefaults.AuthenticationScheme
            : JwtBearerDefaults.AuthenticationScheme;
    })
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
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "otr-session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.IsEssential = true;

        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;

        // By default, cookie-based authentication will try to redirect to a "login" endpoint
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    })
    .AddOAuth("osu", options =>
    {
        // Set cookie scheme to persist user identity
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // This url needs to be added to the osu! OAuth client callback url
        options.CallbackPath = new PathString("/api/v1/auth/callback");

        // Configure osu! OAuth
        // https://osu.ppy.sh/docs/#authorization-code-grant
        OsuConfiguration osuConfig = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position);
        options.ClientId = osuConfig.ClientId.ToString();
        options.ClientSecret = osuConfig.ClientSecret;
        options.Scope.Add("public");
        options.Scope.Add("friends.read");
        options.AuthorizationEndpoint = "https://osu.ppy.sh/oauth/authorize";
        options.TokenEndpoint = "https://osu.ppy.sh/oauth/token";
        options.SaveTokens = true;

        options.Events = new OAuthEvents
        {
            // Event fired when the client successfully gets osu! access credentials for a user
            OnCreatingTicket = async context =>
            {
                IOsuClient osuClient = context.HttpContext.RequestServices.GetRequiredService<IOsuClient>();
                IUsersService usersService = context.HttpContext.RequestServices.GetRequiredService<IUsersService>();

                osuClient.Credentials = new OsuCredentials
                {
                    AccessToken = context.AccessToken ?? string.Empty,
                    RefreshToken = context.RefreshToken,
                    ExpiresInSeconds = (long?)context.ExpiresIn?.TotalSeconds ?? DateTime.Now.Second,
                };

                // Get user data from osu! API
                UserExtended? osuUser = await osuClient.GetCurrentUserAsync(
                    cancellationToken: context.HttpContext.RequestAborted
                );

                if (osuUser is null)
                {
                    return;
                }

                User user = await usersService.LoginAsync(osuUser.Id);

                // Build user identity
                var claims = new List<Claim>
                {
                    new(OtrClaims.Role, OtrClaims.Roles.User),
                    new(OtrClaims.Subject, user.Id.ToString())
                };
                claims.AddRange(user.Scopes.Select(r => new Claim(OtrClaims.Role, r)));

                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(
                    claims: claims,
                    authenticationType: context.Scheme.Name,
                    nameType: OtrClaims.Subject,
                    roleType: OtrClaims.Role
                ));
            }
        };
    });

#endregion

#region Data Protection Configuration

// Use redis to persist keys across application restarts
// This is used for persisting cookies across application restarts
ConnectionStringsConfiguration connectionStrings = builder.Configuration.BindAndValidate<ConnectionStringsConfiguration>(
    ConnectionStringsConfiguration.Position
);

builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(connectionStrings.RedisConnection), "otr-api:data-protection-keys")
    .SetApplicationName("otr-api")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

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
builder.Services.AddDbContext<OtrContext>((services, sqlOptions) =>
{
    sqlOptions
        .UseNpgsql(builder.Configuration.BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).DefaultConnection,
            dataSourceOptions => dataSourceOptions.ConfigureDataSource(
                dataSourceBuilder =>
                    dataSourceBuilder.ConfigureTracing(tracingOptions =>
                    // This only returns the actual command, unfortunately there's no other way to do this afaik
                    tracingOptions.ConfigureCommandSpanNameProvider(cmd => Regex.Split(cmd.CommandText[..15], "[a-z]+")[0])
                            // Filter out commands that shouldn't be related to API operations
                            .ConfigureCommandFilter(cmd => !cmd.CommandText.StartsWith("COMMIT", StringComparison.OrdinalIgnoreCase))
                            .ConfigureCommandFilter(cmd => !cmd.CommandText.StartsWith("DROP", StringComparison.OrdinalIgnoreCase))
                            .ConfigureCommandFilter(cmd => !cmd.CommandText.StartsWith("CREATE", StringComparison.OrdinalIgnoreCase))
                            .ConfigureCommandFilter(cmd => !cmd.CommandText.StartsWith("ALTER", StringComparison.OrdinalIgnoreCase)))))
        .LogTo(Log.Logger.Information, LogLevel.Information)
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

// Redis lock factory (distributed resource access control)
var useRedLock = builder.Configuration.Get<OsuConfiguration>()?.EnableDistributedLocking ?? true;

if (useRedLock)
{
    var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>
    {
        new(ConnectionMultiplexer.Connect(builder.Configuration
            .BindAndValidate<ConnectionStringsConfiguration>(ConnectionStringsConfiguration.Position).RedisConnection))
    });
    builder.Services.AddSingleton(redLockFactory);
}


builder.Services.AddScoped<IPasswordHasher<OAuthClient>, PasswordHasher<OAuthClient>>();

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
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IOAuthClientService, OAuthClientService>();
builder.Services.AddScoped<IPlayerRatingsService, PlayerRatingsService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IFilteringService, FilteringService>();
builder.Services.AddScoped<ITournamentsService, TournamentsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUrlHelperService, UrlHelperService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
builder.Services.AddScoped<IPlatformStatsService, PlatformStatsService>();
builder.Services.AddScoped<ITournamentPlatformStatsService, TournamentPlatformStatsService>();
builder.Services.AddScoped<IRatingPlatformStatsService, RatingPlatformStatsService>();
builder.Services.AddScoped<IUserPlatformStatsService, UserPlatformStatsService>();

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

// After UseCors
app.UseOutputCache();

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
