using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using API.Authorization;
using API.Authorization.Handlers;
using API.Authorization.Requirements;
using API.Configurations;
using API.Handlers.Implementations;
using API.Handlers.Interfaces;
using API.Middlewares;
using API.ModelBinders.Providers;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.Utilities;
using API.Utilities.Extensions;
using Asp.Versioning;
using AutoMapper;
using Dapper;
using Database;
using Database.Entities;
using Database.Repositories.Implementations;
using Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OsuSharp;
using OsuSharp.Extensions;
using Serilog;
using Serilog.Events;

#region WebApplicationBuilder Configuration

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder
    .Services.AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new LeaderboardFilterModelBinderProvider());
    })
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddNewtonsoftJson();

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

    var anonymousRateLimiterOptions = new FixedWindowRateLimiterOptions()
    {
        PermitLimit = 30,
        Window = TimeSpan.FromSeconds(60),
        QueueLimit = 0
    };

    // Configure response for rate limited clients
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        const string readMore =
            "Read more about our rate limits at https://github.com/osu-tournament-rating/otr-wiki/blob/master/api/limits/en.md";
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

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        // Try to get access token
        var accessToken = context
            .Features.Get<IAuthenticateResultFeature>()
            ?.AuthenticateResult?.Properties?.GetTokenValue("access_token")
            ?.ToString();

        // Shared partition for anonymous users
        if (string.IsNullOrEmpty(accessToken))
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                "anonymous",
                _ => anonymousRateLimiterOptions
            );
        }

        JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));

        // Try to parse rate limit override claim
        var overrideClaimValue = principal
            .Claims.FirstOrDefault(c => c.Type == OtrClaims.RateLimitOverrides)
            ?.Value;
        RateLimitOverrides? overrides = null;
        if (!string.IsNullOrEmpty(overrideClaimValue))
        {
            overrides = RateLimitOverridesSerializer.Deserialize(overrideClaimValue);
        }

        // Differentiate between client and users, as they can have the same id
        var prefix = principal.IsUser() ? "user" : "client";

        // Partition for each unique user / client
        if (overrides is null)
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                $"{prefix}_{jwtToken.Issuer}",
                _ => anonymousRateLimiterOptions
            );
        }
        return RateLimitPartition.GetFixedWindowLimiter(
            $"{prefix}_{jwtToken.Issuer}",
            _ => new FixedWindowRateLimiterOptions()
            {
                PermitLimit = overrides.PermitLimit ?? rateLimitConfiguration.PermitLimit,
                Window = TimeSpan.FromSeconds(overrides.Window ?? rateLimitConfiguration.Window),
                QueueLimit = 0
            }
        );
    });
});

#endregion

#region Swagger Configuration

builder
    .Services.AddApiVersioning(options =>
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = true;
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "osu! Tournament Rating API",
            Description = "The official resource for reading and writing data within the osu! Tournament Rating platform.",
            TermsOfService = new Uri(
                "https://github.com/osu-tournament-rating/otr-wiki/blob/master/api/usage/limits/en.md"
            )
        }
    );
    options.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}API.xml");
});

#endregion

#region Logger Configuration

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
        .MinimumLevel.Override("OsuSharp", LogEventLevel.Fatal)
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

builder.Services.AddLogging();

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

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        JwtConfiguration jwtConfiguration = builder.Configuration.BindAndValidate<JwtConfiguration>(
            JwtConfiguration.Position
        );

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = jwtConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfiguration.Key)
            )
        };
    });

#endregion

#region Authorization Configuration

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy(
        AuthorizationPolicies.AccessUserResources,
        policy =>
            policy
                .RequireAuthenticatedUser()
                .AddRequirements(new AccessUserResourcesRequirement(allowSelf: true))
    );

builder.Services.AddSingleton<IAuthorizationHandler, AccessUserResourcesAuthorizationHandler>();

#endregion

#region Dapper

DefaultTypeMap.MatchNamesWithUnderscores = true;

#endregion

#region AutoMapper

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

#endregion

#region Database Context

builder.Services.AddDbContext<OtrContext>(o =>
{
    o.UseNpgsql(
        builder
            .Configuration.BindAndValidate<ConnectionStringsConfiguration>(
                ConnectionStringsConfiguration.Position
            )
            .DefaultConnection
    );
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

builder.Services.AddScoped<IApiBaseStatsRepository, ApiBaseStatsRepository>();
builder.Services.AddScoped<IApiMatchRatingStatsRepository, ApiMatchRatingStatsRepository>();
builder.Services.AddScoped<IApiMatchWinRecordRepository, ApiMatchWinRecordRepository>();
builder.Services.AddScoped<IApiPlayerMatchStatsRepository, ApiPlayerMatchStatsRepository>();
builder.Services.AddScoped<IApiTournamentsRepository, ApiTournamentsRepository>();

builder.Services.AddScoped<IBaseStatsRepository, BaseStatsRepository>();
builder.Services.AddScoped<IBeatmapsRepository, BeatmapsRepository>();
builder.Services.AddScoped<IGamesRepository, GamesRepository>();
builder.Services.AddScoped<IGameWinRecordsRepository, GameWinRecordsRepository>();
builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
builder.Services.AddScoped<IMatchRatingStatsRepository, MatchRatingStatsRepository>();
builder.Services.AddScoped<IGameScoresRepository, GameScoresRepository>();
builder.Services.AddScoped<IMatchWinRecordRepository, MatchWinRecordRepository>();
builder.Services.AddScoped<IOAuthClientRepository, OAuthClientRepository>();
builder.Services.AddScoped<IPlayerMatchStatsRepository, PlayerMatchStatsRepository>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<IRatingAdjustmentsRepository, RatingAdjustmentsRepository>();
builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

#endregion

#region Services

builder.Services.AddScoped<IBaseStatsService, BaseStatsService>();
builder.Services.AddScoped<IBeatmapService, BeatmapService>();
builder.Services.AddScoped<IGameWinRecordsService, GameWinRecordsService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IOAuthClientService, OAuthClientService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddScoped<IRatingAdjustmentsService, RatingAdjustmentsesService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IScreeningService, ScreeningService>();
builder.Services.AddScoped<ITournamentsService, TournamentsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUrlHelperService, UrlHelperService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();

#endregion

#region osu! Api

builder.Services.AddOsuSharp(options =>
{
    OsuConfiguration osuConfiguration = builder.Configuration.BindAndValidate<OsuConfiguration>(
        OsuConfiguration.Position
    );
    options.Configuration = new OsuClientConfiguration
    {
        ClientId = osuConfiguration.ClientId,
        ClientSecret = osuConfiguration.ClientSecret
    };
});

builder.Host.ConfigureOsuSharp(
    (_, options) =>
    {
        OsuConfiguration osuConfiguration = builder.Configuration.BindAndValidate<OsuConfiguration>(
            OsuConfiguration.Position
        );

        options.Configuration = new OsuClientConfiguration
        {
            ClientId = osuConfiguration.ClientId,
            ClientSecret = osuConfiguration.ClientSecret
        };
    }
);

#endregion

#endregion

#region WebApplication Configuration

WebApplication app = builder.Build();

// Set switch for Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.UseRouting(); // UseRouting must come first before UseCors
app.UseCors(); // Placed after UseRouting and before UseAuthentication and UseAuthorization

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();

IOptions<AuthConfiguration> authConfiguration = app.Services.GetRequiredService<
    IOptions<AuthConfiguration>
>();
if (authConfiguration.Value.EnforceWhitelist)
{
    app.UseMiddleware<WhitelistEnforcementMiddleware>();
}

app.UseAuthorization();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    // only during development, validate your mappings
    IMapper mapper = app.Services.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();

    app.UseSwagger();
    app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "osu! Tournament Rating API"); });
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

app.Logger.LogInformation("Running!");

#region Database Migrations

using IServiceScope scope = app.Services.CreateScope();
OtrContext context = scope.ServiceProvider.GetRequiredService<OtrContext>();

var migrationsCount = context.Database.GetPendingMigrations().Count();
if (migrationsCount > 0)
{
    await context.Database.MigrateAsync();
    app.Logger.LogInformation("Applied {MigrationsCount} pending migration(s).", migrationsCount);
}

#endregion

#endregion

app.Run();
