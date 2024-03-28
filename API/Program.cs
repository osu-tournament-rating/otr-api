using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using API;
using API.BackgroundWorkers;
using API.Configurations;
using API.Entities;
using API.Handlers.Implementations;
using API.Handlers.Interfaces;
using API.ModelBinders.Providers;
using API.Osu.Multiplayer;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OsuSharp;
using OsuSharp.Extensions;
using Serilog;
using Serilog.Events;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configurations
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

// Add services to the container.

builder
    .Services.AddControllers(options => { options.ModelBinderProviders.Insert(0, new LeaderboardFilterModelBinderProvider()); })
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddNewtonsoftJson();

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
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder
    .Services.AddRateLimiter(options =>
    {
        RateLimitConfiguration rateLimitConfiguration = builder.Configuration.BindAndValidate<RateLimitConfiguration>(
            RateLimitConfiguration.Position);

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
                    $"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s). " + readMore,
                    token
                );
            }
            else
            {
                await context.HttpContext.Response.WriteAsync(
                    "Too many requests. Please try again later. " + readMore, token);
            }
        };

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            // Try to get access token
            var accessToken =
                context.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult?.Properties
                    ?.GetTokenValue("access_token")?.ToString();

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
            var overrideClaimValue = principal.Claims
                .FirstOrDefault(c => c.Type == OtrClaimTypes.RateLimitOverrides)?.Value;
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.OperationFilter<HttpResultsOperationFilter>(); });

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
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
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

DefaultTypeMap.MatchNamesWithUnderscores = true;

var configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MapperProfile>(); });

// only during development, validate your mappings; remove it before release
#if DEBUG
configuration.AssertConfigurationIsValid();
#endif

builder.Services.AddSingleton(configuration.CreateMapper());

builder.Services.AddLogging();

// Hosted services
builder.Services.AddHostedService<MatchDuplicateDataWorker>();
builder.Services.AddHostedService<OsuPlayerDataWorker>();
builder.Services.AddHostedService<OsuMatchDataWorker>();
builder.Services.AddHostedService<OsuTrackApiWorker>();

// Handlers
builder.Services.AddScoped<IOAuthHandler, OAuthHandler>();

// Database context
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

builder.Services.AddDistributedMemoryCache();

// Repositories
builder.Services.AddScoped<IApiMatchRepository, ApiMatchRepository>();
builder.Services.AddScoped<IBaseStatsRepository, BaseStatsRepository>();
builder.Services.AddScoped<IBeatmapRepository, BeatmapRepository>();
builder.Services.AddScoped<IGamesRepository, GamesRepository>();
builder.Services.AddScoped<IGameWinRecordsRepository, GameWinRecordsRepository>();
builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
builder.Services.AddScoped<IMatchDuplicateRepository, MatchDuplicateRepository>();
builder.Services.AddScoped<IMatchRatingStatsRepository, MatchRatingStatsRepository>();
builder.Services.AddScoped<IMatchScoresRepository, MatchScoresRepository>();
builder.Services.AddScoped<IMatchWinRecordsRepository, MatchWinRecordsRepository>();
builder.Services.AddScoped<IOAuthClientRepository, OAuthClientRepository>();
builder.Services.AddScoped<IPlayerMatchStatsRepository, PlayerMatchStatsRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IRatingAdjustmentsRepository, RatingAdjustmentsRepository>();
builder.Services.AddScoped<ITournamentsRepository, TournamentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IBaseStatsService, BaseStatsService>();
builder.Services.AddScoped<IBeatmapService, BeatmapService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IOAuthClientService, OAuthClientService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ITournamentsService, TournamentsService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddOsuSharp(options =>
{
    OsuConfiguration osuConfiguration = builder.Configuration.BindAndValidate<OsuConfiguration>(OsuConfiguration.Position);
    options.Configuration = new OsuClientConfiguration { ClientId = osuConfiguration.ClientId, ClientSecret = osuConfiguration.ClientSecret };
});

builder.Services.AddSingleton<IOsuApiService, OsuApiService>();

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

builder.Host.ConfigureOsuSharp(
    (_, options) =>
    {
        OsuConfiguration osuConfiguration = builder.Configuration.BindAndValidate<OsuConfiguration>(
            OsuConfiguration.Position
        );

        options.Configuration = new OsuClientConfiguration { ClientId = osuConfiguration.ClientId, ClientSecret = osuConfiguration.ClientSecret };
    }
);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        JwtConfiguration jwtConfiguration =
            builder.Configuration.BindAndValidate<JwtConfiguration>(JwtConfiguration.Position);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = jwtConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key)),
        };
    });

WebApplication app = builder.Build();

// Set switch for Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // UseRouting must come first before UseCors

app.UseCors(); // Placed after UseRouting and before UseAuthentication and UseAuthorization

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

app.Logger.LogInformation("Running!");

// Migrations
using IServiceScope scope = app.Services.CreateScope();
OtrContext context = scope.ServiceProvider.GetRequiredService<OtrContext>();

var migrationsCount = context.Database.GetPendingMigrations().Count();
if (migrationsCount > 0)
{
    await context.Database.MigrateAsync();
    app.Logger.LogInformation("Applied {MigrationsCount} pending migration(s).", migrationsCount);
}

app.Run();
